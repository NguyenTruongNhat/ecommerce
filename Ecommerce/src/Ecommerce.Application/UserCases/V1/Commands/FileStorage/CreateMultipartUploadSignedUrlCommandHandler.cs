using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

/// <summary>
/// Handler for generating presigned URLs for individual parts in a multipart upload
/// This allows clients to upload large files in chunks directly to S3
/// </summary>
public sealed class CreateMultipartUploadSignedUrlCommandHandler
    : ICommandHandler<Command.CreateMultipartUploadSignedUrlCommand, Response.MultipartUploadSignedUrlResponseDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<CreateMultipartUploadSignedUrlCommandHandler> _logger;
    private const int DefaultExpirationMinutes = 60;
    private const int MaxPartNumber = 10000;

    public CreateMultipartUploadSignedUrlCommandHandler(
        IFileStorageService fileStorageService,
        ILogger<CreateMultipartUploadSignedUrlCommandHandler> logger)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Response.MultipartUploadSignedUrlResponseDto>> Handle(
        Command.CreateMultipartUploadSignedUrlCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Generating presigned URL for multipart upload part. UploadId: {UploadId}, ObjectName: {ObjectName}, PartNumber: {PartNumber}",
                request.UploadId, request.ObjectName, request.PartNumber);

            // Validate request
            var validationResult = ValidateRequest(request);
            if (validationResult.IsFailure)
            {
                return (Result<Response.MultipartUploadSignedUrlResponseDto>)Result<Response.MultipartUploadSignedUrlResponseDto>.Failure(validationResult.Error);
            }

            // Generate presigned URL for this specific part
            var signedUrl = await _fileStorageService.GeneratePresignedUrlForPartAsync(
                request.ObjectName,
                request.UploadId,
                request.PartNumber,
                expiresInMinutes: DefaultExpirationMinutes,
                cancellationToken);

            var expiresAt = DateTime.UtcNow.AddMinutes(DefaultExpirationMinutes);

            var response = new Response.MultipartUploadSignedUrlResponseDto
            {
                SignedUrl = signedUrl,
                UploadId = request.UploadId,
                ObjectName = request.ObjectName,
                PartNumber = request.PartNumber,
                ExpiresInMinutes = DefaultExpirationMinutes,
                ExpiresAt = expiresAt
            };

            _logger.LogInformation(
                "Successfully generated presigned URL for part {PartNumber}/{MaxParts}. UploadId: {UploadId}, Expires at: {ExpiresAt}",
                request.PartNumber, MaxPartNumber, request.UploadId, expiresAt);

            // Log progress for tracking
            LogUploadProgress(request.PartNumber, request.UploadId, request.ObjectName);

            return Result<Response.MultipartUploadSignedUrlResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to generate presigned URL for multipart upload part. UploadId: {UploadId}, PartNumber: {PartNumber}",
                request.UploadId, request.PartNumber);

            return (Result<Response.MultipartUploadSignedUrlResponseDto>)Result<Response.MultipartUploadSignedUrlResponseDto>.Failure(
                new Error(
                    "FileStorage.PresignedUrlGenerationFailed",
                    $"Failed to generate presigned URL for part {request.PartNumber}: {ex.Message}"));
        }
    }

    /// <summary>
    /// Validates the multipart upload signed URL request
    /// </summary>
    private Result ValidateRequest(Command.CreateMultipartUploadSignedUrlCommand request)
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

        if (request.PartNumber < 1 || request.PartNumber > MaxPartNumber)
        {
            _logger.LogWarning(
                "Invalid part number: {PartNumber}. Must be between 1 and {MaxPartNumber}",
                request.PartNumber, MaxPartNumber);

            return Result.Failure(
                new Error(
                    "FileStorage.InvalidPartNumber",
                    $"PartNumber must be between 1 and {MaxPartNumber}"));
        }

        return Result.Success();
    }

    /// <summary>
    /// Logs upload progress for monitoring and debugging
    /// This simulates progress tracking - in production, you might track this in a database or cache
    /// </summary>
    private void LogUploadProgress(int partNumber, string uploadId, string objectName)
    {
        // Calculate approximate progress based on part number
        // Note: This is a rough estimate since we don't know the total number of parts yet
        var estimatedProgress = Math.Min(partNumber * 100.0 / 100, 100); // Assuming ~100 parts max for logging

        _logger.LogInformation(
            "[Upload Progress] UploadId: {UploadId}, ObjectName: {ObjectName}, Part: {PartNumber}, Estimated Progress: {Progress:F2}%",            uploadId, objectName, partNumber, estimatedProgress);

        // Additional detailed logging for debugging
        _logger.LogDebug(
            "[Multipart Upload Details] UploadId: {UploadId} | Part: {PartNumber} | Status: URL Generated | Timestamp: {Timestamp}",
            uploadId, partNumber, DateTime.UtcNow);
    }
}
