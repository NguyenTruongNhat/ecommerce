using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Constants;
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
        { ".jpg", "image/jpeg" }, { ".jpeg", "image/jpeg" }, { ".png", "image/png" },
        { ".gif", "image/gif" }, { ".bmp", "image/bmp" }, { ".webp", "image/webp" },
        { ".svg", "image/svg+xml" }, { ".ico", "image/x-icon" }, { ".pdf", "application/pdf" },
        { ".doc", "application/msword" }, { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" }, { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".ppt", "application/vnd.ms-powerpoint" }, { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        { ".txt", "text/plain" }, { ".csv", "text/csv" }, { ".mp4", "video/mp4" },
        { ".avi", "video/x-msvideo" }, { ".mov", "video/quicktime" }, { ".wmv", "video/x-ms-wmv" },
        { ".flv", "video/x-flv" }, { ".webm", "video/webm" }, { ".mp3", "audio/mpeg" },
        { ".wav", "audio/wav" }, { ".ogg", "audio/ogg" }, { ".zip", "application/zip" },
        { ".rar", "application/x-rar-compressed" }, { ".7z", "application/x-7z-compressed" },
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

    public async Task<string> InitiateMultipartUploadAsync(
        string objectName,
        CancellationToken cancellationToken = default)
    {

        _logger.LogInformation("Initiating multipart upload. ObjectName: {ObjectName}", objectName);

        var request = new InitiateMultipartUploadRequest
        {
            BucketName = _options.BucketName,
            Key = objectName,
        };

        var response = await _s3Client.InitiateMultipartUploadAsync(request, cancellationToken);

        _logger.LogInformation("Multipart upload initiated successfully. UploadId: {UploadId}, ObjectName: {ObjectName}", response.UploadId, objectName);

        return response.UploadId;
    }

    public async Task<string> GeneratePresignedUrlForPartAsync(
        string objectName,
        string uploadId,
        int partNumber)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = objectName,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddSeconds(FileStorageVariables.SignedURLLimitedTime),
            Parameters =
            {
                ["uploadId"] = uploadId,
                ["partNumber"] = partNumber.ToString()
            }
        };
        var presignedUrl = await _s3Client.GetPreSignedURLAsync(request);
        return presignedUrl;
    }

    public async Task<string> UploadFileAsync(
        IFormFile file,
        string objectName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(file);
        ValidateObjectName(objectName);
        ValidateContentType(contentType);

        _logger.LogInformation(
            "Starting direct upload to S3. ObjectName: {ObjectName}, FileName: {FileName}, FileSize: {FileSize} bytes",
            objectName, file.FileName, file.Length);

        using var stream = file.OpenReadStream();
        var putRequest = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectName,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = false,
            Metadata =
            {
                ["original-filename"] = file.FileName,
                ["uploaded-at"] = DateTime.UtcNow.ToString("O")
            }
        };

        var response = await _s3Client.PutObjectAsync(putRequest, cancellationToken);

        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            throw new InvalidOperationException(
                $"Failed to upload file to S3. Status code: {response.HttpStatusCode}");

        _logger.LogInformation(
            "Successfully uploaded file to S3. ObjectName: {ObjectName}, ETag: {ETag}",
            objectName, response.ETag);

        return $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{objectName}";
    }

    public async Task<string> UploadFileWithProgressAsync(
        IFormFile file,
        string objectName,
        string contentType,
        Action<long, long> progressCallback,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(file);
        ValidateObjectName(objectName);
        ValidateContentType(contentType);

        _logger.LogInformation(
            "Starting upload with progress tracking. ObjectName: {ObjectName}, FileSize: {FileSize} bytes",
            objectName, file.Length);

        using var transferUtility = new TransferUtility(_s3Client);
        using var stream = file.OpenReadStream();

        var uploadRequest = new TransferUtilityUploadRequest
        {
            BucketName = _options.BucketName,
            Key = objectName,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = false,
            PartSize = 6 * 1024 * 1024,
            Metadata =
            {
                ["original-filename"] = file.FileName,
                ["uploaded-at"] = DateTime.UtcNow.ToString("O")
            }
        };

        uploadRequest.UploadProgressEvent += (sender, args) =>
        {
            progressCallback?.Invoke(args.TransferredBytes, args.TotalBytes);
        };

        await transferUtility.UploadAsync(uploadRequest, cancellationToken);

        _logger.LogInformation(
            "Successfully uploaded file with progress tracking. ObjectName: {ObjectName}",
            objectName);

        return $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{objectName}";
    }

    public async Task<string> GeneratePresignedUploadUrlAsync(
        string objectName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = objectName,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddSeconds(FileStorageVariables.SignedURLLimitedTime),
            ContentType = contentType
        };
        var presignedUrl = await _s3Client.GetPreSignedURLAsync(request);
        return presignedUrl;
    }

    public async Task<string> GeneratePresignedDownloadUrlAsync(
        string objectName,
        int expiresInMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        ValidateObjectName(objectName);
        if (expiresInMinutes <= 0 || expiresInMinutes > 10080)
            throw new ArgumentException("Expiration time must be between 1 and 10080 minutes", nameof(expiresInMinutes));

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

    public async Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(
        string objectName,
        string uploadId,
        List<PartETag> partETags,
        CancellationToken cancellationToken = default)
    {

        var sortedPartETags = partETags.OrderBy(p => p.PartNumber).ToList();

        var request = new CompleteMultipartUploadRequest
        {
            BucketName = _options.BucketName,
            Key = objectName,
            UploadId = uploadId,
            PartETags = sortedPartETags
        };

        var response = await _s3Client.CompleteMultipartUploadAsync(request, cancellationToken);

        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            throw new InvalidOperationException(
                $"Failed to complete multipart upload. Status code: {response.HttpStatusCode}");

        return response;
    }

    public async Task AbortMultipartUploadAsync(
        string objectName,
        string uploadId,
        CancellationToken cancellationToken = default)
    {
        var request = new AbortMultipartUploadRequest
        {
            BucketName = _options.BucketName,
            Key = objectName,
            UploadId = uploadId
        };
        var response = await _s3Client.AbortMultipartUploadAsync(request, cancellationToken);

        if (response.HttpStatusCode != System.Net.HttpStatusCode.NoContent)
        {
            _logger.LogWarning("Multipart upload abort returned unexpected status code: {StatusCode}", response.HttpStatusCode);
        }
    }

    public string GenerateObjectName(string appServiceName, string fileName, string uploadingProgressId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

        var sanitizedAppServiceName = SanitizePath(appServiceName ?? "Default");
        var sanitizedProgressId = SanitizePath(uploadingProgressId ?? Guid.NewGuid().ToString());
        var extension = Path.GetExtension(fileName);
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
        var sanitizedFileName = SanitizeFileName(fileNameWithoutExt);
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

    private static void ValidateObjectName(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
            throw new ArgumentException("Object name cannot be null or empty", nameof(objectName));
    }

    private static void ValidateContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be null or empty", nameof(contentType));
    }

    private static void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File cannot be null or empty", nameof(file));
    }

    private static string SanitizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "default";

        var invalidChars = new[] { '\\', '<', '>', ':', '"', '|', '?', '*' };
        var sanitized = path;

        foreach (var c in invalidChars)
            sanitized = sanitized.Replace(c, '_');

        return sanitized.Trim('/', ' ');
    }

    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "file";

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = fileName;

        foreach (var c in invalidChars)
            sanitized = sanitized.Replace(c, '_');

        sanitized = sanitized.Replace(' ', '_');

        return sanitized;
    }
}
