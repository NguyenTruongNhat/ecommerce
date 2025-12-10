using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

/// <summary>
/// Handler for completing a multipart upload to S3
/// This combines all uploaded parts into a single object and finalizes the upload session
/// </summary>
public sealed class CompleteMultipartUploadCommandHandler
    : ICommandHandler<Command.CompleteMultipartUploadCommand, Response.CompleteMultipartUploadResponseDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<CompleteMultipartUploadCommandHandler> _logger;

    public CompleteMultipartUploadCommandHandler(
        IFileStorageService fileStorageService,
        ILogger<CompleteMultipartUploadCommandHandler> logger)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Response.CompleteMultipartUploadResponseDto>> Handle(
        Command.CompleteMultipartUploadCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Completing multipart upload. UploadId: {UploadId}, ObjectName: {ObjectName}, Parts: {PartCount}",
                request.UploadId, request.ObjectName, request.PartETags?.Count ?? 0);

            // Validate request
            var validationResult = ValidateRequest(request);
            if (validationResult.IsFailure)
            {
                return (Result<Response.CompleteMultipartUploadResponseDto>)Result<Response.CompleteMultipartUploadResponseDto>.Failure(validationResult.Error);
            }

            // Log all parts for debugging
            LogPartDetails(request.UploadId, request.PartETags);

            // Complete the multipart upload in S3
            var s3Response = await _fileStorageService.CompleteMultipartUploadAsync(
                request.ObjectName,
                request.UploadId,
                request.PartETags,
                cancellationToken);

            // Generate a presigned download URL (valid for 7 days)
            var downloadUrl = await _fileStorageService.GeneratePresignedDownloadUrlAsync(
                request.ObjectName,
                expiresInMinutes: 10080, // 7 days
                cancellationToken);

            var response = new Response.CompleteMultipartUploadResponseDto
            {
                ObjectName = request.ObjectName,
                UploadId = request.UploadId,
                ETag = s3Response.ETag,
                Location = s3Response.Location,
                TotalParts = request.PartETags.Count,
                DownloadUrl = downloadUrl,
                CompletedAt = DateTime.UtcNow
            };

            _logger.LogInformation(
                "Multipart upload completed successfully. UploadId: {UploadId}, ObjectName: {ObjectName}, ETag: {ETag}, TotalParts: {TotalParts}",
                request.UploadId, request.ObjectName, s3Response.ETag, request.PartETags.Count);

            return Result<Response.CompleteMultipartUploadResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to complete multipart upload. UploadId: {UploadId}, ObjectName: {ObjectName}",
                request.UploadId, request.ObjectName);

            return (Result<Response.CompleteMultipartUploadResponseDto>)Result<Response.CompleteMultipartUploadResponseDto>.Failure(
                new Error(
                    "FileStorage.CompleteMultipartFailed",
                    $"Failed to complete multipart upload: {ex.Message}"));
        }
    }

    /// <summary>
    /// Validates the complete multipart upload request
    /// </summary>
    private Result ValidateRequest(Command.CompleteMultipartUploadCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.UploadId))
        {
            _logger.LogWarning("UploadId is required but was not provided");
            return Result.Failure(
                new Error("FileStorage.InvalidRequest", "UploadId is required"));
        }

        if (string.IsNullOrWhiteSpace(request.ObjectName))
        {
            _logger.LogWarning("ObjectName is required but was not provided");
            return Result.Failure(
                new Error("FileStorage.InvalidRequest", "ObjectName is required"));
        }

        if (request.PartETags == null || request.PartETags.Count == 0)
        {
            _logger.LogWarning("PartETags list is empty");
            return Result.Failure(
                new Error("FileStorage.InvalidRequest", "At least one part ETag is required"));
        }

        // Validate part numbers are sequential and start from 1
        var sortedParts = request.PartETags.OrderBy(p => p.PartNumber).ToList();
        for (int i = 0; i < sortedParts.Count; i++)
        {
            if (sortedParts[i].PartNumber != i + 1)
            {
                _logger.LogWarning(
                    "Invalid part numbering. Expected part {Expected}, found part {Actual}",
                    i + 1, sortedParts[i].PartNumber);

                return Result.Failure(
                    new Error(
                        "FileStorage.InvalidPartNumbers",
                        "Part numbers must be sequential starting from 1"));
            }

            if (string.IsNullOrWhiteSpace(sortedParts[i].ETag))
            {
                _logger.LogWarning("Part {PartNumber} has empty ETag", sortedParts[i].PartNumber);
                return Result.Failure(
                    new Error(
                        "FileStorage.InvalidPartETag",
                        $"Part {sortedParts[i].PartNumber} has invalid ETag"));
            }
        }

        return Result.Success();
    }

    /// <summary>
    /// Logs detailed information about all uploaded parts
    /// </summary>
    private void LogPartDetails(string uploadId, List<Amazon.S3.Model.PartETag> partETags)
    {
        _logger.LogInformation(
            "Part details for UploadId: {UploadId}. Total parts: {TotalParts}",
            uploadId, partETags.Count);

        var sortedParts = partETags.OrderBy(p => p.PartNumber).ToList();
        
        foreach (var part in sortedParts)
        {
            _logger.LogDebug(
                "Part {PartNumber}: ETag={ETag}",
                part.PartNumber, part.ETag);
        }

        _logger.LogInformation(
            "All parts validated. Ready to complete upload for UploadId: {UploadId}",
            uploadId);
    }
}
