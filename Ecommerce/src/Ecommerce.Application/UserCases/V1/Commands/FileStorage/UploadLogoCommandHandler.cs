using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

/// <summary>
/// Handler for uploading logo files directly to S3 storage
/// This performs server-side upload, unlike signed URL approach where client uploads directly
/// </summary>
public sealed class UploadLogoCommandHandler 
    : ICommandHandler<Command.UploadLogoCommand, Response.UploadLogoResponseDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<UploadLogoCommandHandler> _logger;

    private const string LogoAppServiceName = "Logos";
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5MB
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".svg", ".webp"];

    public UploadLogoCommandHandler(
        IFileStorageService fileStorageService,
        ILogger<UploadLogoCommandHandler> logger)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Response.UploadLogoResponseDto>> Handle(
        Command.UploadLogoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Processing logo upload for ResourceId: {ResourceId}, FileName: {FileName}",
                request.ResourceId, request.File?.FileName);

            // Validate the uploaded file
            var validationResult = ValidateFile(request.File);
            if (validationResult.IsFailure)
            {
                return Result<Response.UploadLogoResponseDto>.Failure(validationResult.Error);
            }

            // Use the provided filename or fall back to the uploaded file's name
            var fileName = string.IsNullOrWhiteSpace(request.FileName)
                ? request.File.FileName
                : request.FileName;

            // Generate unique object name using ResourceId as the uploading progress ID
            // This creates a path like: Logos/{ResourceId}/{timestamp}_{filename}.ext
            var objectName = _fileStorageService.GenerateObjectName(
                LogoAppServiceName,
                fileName,
                request.ResourceId);

            // Determine content type
            var contentType = _fileStorageService.GetContentType(fileName);

            _logger.LogInformation(
                "Uploading logo to S3. ObjectName: {ObjectName}, ContentType: {ContentType}, Size: {Size} bytes",
                objectName, contentType, request.File.Length);

            // Upload file directly to S3
            var objectUrl = await _fileStorageService.UploadFileAsync(
                request.File,
                objectName,
                contentType,
                cancellationToken);

            // Generate a presigned download URL (valid for 7 days)
            var downloadUrl = await _fileStorageService.GeneratePresignedDownloadUrlAsync(
                objectName,
                expiresInMinutes: 10080, // 7 days
                cancellationToken);

            var response = new Response.UploadLogoResponseDto
            {
                ResourceId = request.ResourceId,
                ObjectName = objectName,
                FileName = fileName,
                ContentType = contentType,
                FileSize = request.File.Length,
                DownloadUrl = downloadUrl,
                UploadedAt = DateTime.UtcNow
            };

            _logger.LogInformation(
                "Successfully uploaded logo for ResourceId: {ResourceId}, ObjectName: {ObjectName}",
                request.ResourceId, objectName);

            return Result<Response.UploadLogoResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to upload logo for ResourceId: {ResourceId}",
                request.ResourceId);

            return Result<Response.UploadLogoResponseDto>.Failure(
                new Error(
                    "FileStorage.UploadFailed",
                    $"Failed to upload logo: {ex.Message}"));
        }
    }

    /// <summary>
    /// Validates the uploaded file for security and business rules
    /// </summary>
    private Result ValidateFile(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        // Check if file exists
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("File validation failed: File is null or empty");
            return Result.Failure(
                new Error("FileStorage.InvalidFile", "File is required and cannot be empty"));
        }

        // Check file size
        if (file.Length > MaxFileSizeInBytes)
        {
            _logger.LogWarning(
                "File validation failed: File size {Size} exceeds maximum allowed size {MaxSize}",
                file.Length, MaxFileSizeInBytes);

            return Result.Failure(
                new Error(
                    "FileStorage.FileTooLarge",
                    $"File size exceeds maximum allowed size of {MaxFileSizeInBytes / 1024 / 1024}MB"));
        }

        // Check file extension
        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            _logger.LogWarning(
                "File validation failed: Invalid file extension {Extension}",
                extension);

            return Result.Failure(
                new Error(
                    "FileStorage.InvalidFileType",
                    $"Only image files are allowed. Supported formats: {string.Join(", ", AllowedExtensions)}"));
        }

        // Additional security: Check file content type
        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "File validation failed: Invalid content type {ContentType}",
                file.ContentType);

            return Result.Failure(
                new Error(
                    "FileStorage.InvalidContentType",
                    "File must be an image"));
        }

        return Result.Success();
    }
}
