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

    /// <summary>
    /// Logs detailed information about all uploaded parts
    /// </summary>
    private void LogPartDetails(string uploadId, List<Amazon.S3.Model.PartETag> partETags)
    {
        _logger.LogInformation(            "Part details for UploadId: {UploadId}. Total parts: {TotalParts}",            uploadId, partETags.Count);

        var sortedParts = partETags.OrderBy(p => p.PartNumber).ToList();

        foreach (var part in sortedParts)
        {
            _logger.LogDebug(
                "Part {PartNumber}: ETag={ETag}",
                part.PartNumber, part.ETag);
        }

        _logger.LogInformation(            "All parts validated. Ready to complete upload for UploadId: {UploadId}",            uploadId);
    }
}
