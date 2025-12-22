using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Helper;
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
        // Generate presigned URL for this specific part
        var signedUrl = await _fileStorageService.GeneratePresignedUrlForPartAsync(request.ObjectName, request.UploadId, request.PartNumber);

        var expiresAt = DateTime.UtcNow.AddMinutes(DefaultExpirationMinutes);

        var response = new Response.MultipartUploadSignedUrlResponseDto
        {
            SignedUrl = signedUrl,
            ObjectName = request.ObjectName,
            ContentType = FileStorageFunctionHelper.GetUploadContentType(request.FileName),
        };

        // Log progress for tracking
        LogUploadProgress(request.PartNumber, request.UploadId, request.ObjectName);

        return Result<Response.MultipartUploadSignedUrlResponseDto>.Success(response);
    }


    /// <summary>
    /// Logs upload progress for monitoring and debugging
    /// This simulates progress tracking - in production, you might track this in a database or cache
    /// </summary>
    private void LogUploadProgress(int partNumber, string uploadId, string objectName)
    {
        // Calculate approximate progress based on part number
        var estimatedProgress = Math.Min(partNumber * 100.0 / 100, 100); // Assuming ~100 parts max for logging

        _logger.LogInformation(
            "[Upload Progress] UploadId: {UploadId}, ObjectName: {ObjectName}, Part: {PartNumber}, Estimated Progress: {Progress:F2}%",
            uploadId, objectName, partNumber, estimatedProgress);

        _logger.LogDebug(
            "[Multipart Upload Details] UploadId: {UploadId} | Part: {PartNumber} | Status: URL Generated | Timestamp: {Timestamp}",
            uploadId, partNumber, DateTime.UtcNow);
    }
}
