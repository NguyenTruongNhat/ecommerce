using Amazon.S3;
using Amazon.S3.Model;
using Ecommerce.Application.Abstractions;
using Ecommerce.Infrastructure.DependencyInjection.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure.Storage;

/// <summary>
/// AWS S3 implementation of file storage service
/// </summary>
public sealed class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly AwsS3Options _options;
    private readonly ILogger<S3FileStorageService> _logger;

    private static readonly Dictionary<string, string> ContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        // Images
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".bmp", "image/bmp" },
        { ".webp", "image/webp" },
        { ".svg", "image/svg+xml" },
        { ".ico", "image/x-icon" },
        
        // Documents
        { ".pdf", "application/pdf" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".ppt", "application/vnd.ms-powerpoint" },
        { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        { ".txt", "text/plain" },
        { ".csv", "text/csv" },
        
        // Videos
        { ".mp4", "video/mp4" },
        { ".avi", "video/x-msvideo" },
        { ".mov", "video/quicktime" },
        { ".wmv", "video/x-ms-wmv" },
        { ".flv", "video/x-flv" },
        { ".webm", "video/webm" },
        
        // Audio
        { ".mp3", "audio/mpeg" },
        { ".wav", "audio/wav" },
        { ".ogg", "audio/ogg" },
        
        // Archives
        { ".zip", "application/zip" },
        { ".rar", "application/x-rar-compressed" },
        { ".7z", "application/x-7z-compressed" },
        
        // Default
        { "", "application/octet-stream" }
    };

    public S3FileStorageService(
        IAmazonS3 s3Client,
        IOptions<AwsS3Options> options,
        ILogger<S3FileStorageService> logger)
    {
        _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> UploadFileAsync(
        IFormFile file,
        string objectName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File cannot be null or empty", nameof(file));

        if (string.IsNullOrWhiteSpace(objectName))
            throw new ArgumentException("Object name cannot be null or empty", nameof(objectName));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be null or empty", nameof(contentType));

        try
        {
            _logger.LogInformation(
                "Starting direct upload to S3. ObjectName: {ObjectName}, FileName: {FileName}, FileSize: {FileSize} bytes",
                objectName, file.FileName, file.Length);

            // Create the PutObject request
            using var stream = file.OpenReadStream();
            var putRequest = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = objectName,
                InputStream = stream,
                ContentType = contentType,
                AutoCloseStream = false,
                // Set metadata
                Metadata =
                {
                    ["original-filename"] = file.FileName,
                    ["uploaded-at"] = DateTime.UtcNow.ToString("O")
                }
            };

            // Upload the file
            var response = await _s3Client.PutObjectAsync(putRequest, cancellationToken);

            // Verify upload success
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new InvalidOperationException(
                    $"Failed to upload file to S3. Status code: {response.HttpStatusCode}");
            }

            _logger.LogInformation(
                "Successfully uploaded file to S3. ObjectName: {ObjectName}, ETag: {ETag}",
                objectName, response.ETag);

            // Return the object URL (public URL format)
            var objectUrl = $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{objectName}";
            return objectUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to upload file to S3. ObjectName: {ObjectName}, FileName: {FileName}",
                objectName, file.FileName);
            throw;
        }
    }

    public async Task<string> GeneratePresignedUploadUrlAsync(
        string objectName,
        string contentType,
        int expiresInMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectName))
            throw new ArgumentException("Object name cannot be null or empty", nameof(objectName));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be null or empty", nameof(contentType));

        if (expiresInMinutes <= 0 || expiresInMinutes > 10080) // Max 7 days
            throw new ArgumentException("Expiration time must be between 1 and 10080 minutes", nameof(expiresInMinutes));

        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = objectName,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                ContentType = contentType
            };

            var presignedUrl = await _s3Client.GetPreSignedURLAsync(request);

            _logger.LogInformation(
                "Generated presigned upload URL for object: {ObjectName}, ContentType: {ContentType}, Expires in: {Minutes} minutes",
                objectName, contentType, expiresInMinutes);

            return presignedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate presigned upload URL for object: {ObjectName}", objectName);
            throw;
        }
    }

    public async Task<string> GeneratePresignedDownloadUrlAsync(
        string objectName,
        int expiresInMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectName))
            throw new ArgumentException("Object name cannot be null or empty", nameof(objectName));

        if (expiresInMinutes <= 0 || expiresInMinutes > 10080) // Max 7 days
            throw new ArgumentException("Expiration time must be between 1 and 10080 minutes", nameof(expiresInMinutes));

        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = objectName,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes)
            };

            var presignedUrl = await _s3Client.GetPreSignedURLAsync(request);

            _logger.LogInformation(
                "Generated presigned download URL for object: {ObjectName}, Expires in: {Minutes} minutes",
                objectName, expiresInMinutes);

            return presignedUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate presigned download URL for object: {ObjectName}", objectName);
            throw;
        }
    }

    public string GenerateObjectName(string appServiceName, string fileName, string uploadingProgressId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

        // Sanitize inputs
        var sanitizedAppServiceName = SanitizePath(appServiceName ?? "Default");
        var sanitizedProgressId = SanitizePath(uploadingProgressId ?? Guid.NewGuid().ToString());
        
        // Extract file extension
        var extension = Path.GetExtension(fileName);
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
        var sanitizedFileName = SanitizeFileName(fileNameWithoutExt);

        // Generate unique object name with structure: appServiceName/uploadingProgressId/timestamp_filename.ext
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var uniqueFileName = $"{timestamp}_{sanitizedFileName}{extension}";
        
        var objectName = $"{sanitizedAppServiceName}/{sanitizedProgressId}/{uniqueFileName}";

        _logger.LogDebug("Generated object name: {ObjectName} from fileName: {FileName}", objectName, fileName);

        return objectName;
    }

    public string GetContentType(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return ContentTypes[""];

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        
        return ContentTypes.TryGetValue(extension, out var contentType)
            ? contentType
            : ContentTypes[""];
    }

    private static string SanitizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "default";

        // Remove invalid characters for S3 paths
        var invalidChars = new[] { '\\', '<', '>', ':', '"', '|', '?', '*' };
        var sanitized = path;

        foreach (var c in invalidChars)
        {
            sanitized = sanitized.Replace(c, '_');
        }

        // Remove leading/trailing slashes and spaces
        return sanitized.Trim('/', ' ');
    }

    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "file";

        // Remove invalid characters
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = fileName;

        foreach (var c in invalidChars)
        {
            sanitized = sanitized.Replace(c, '_');
        }

        // Replace spaces with underscores
        sanitized = sanitized.Replace(' ', '_');

        return sanitized;
    }
}
