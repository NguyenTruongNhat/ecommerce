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
        // Use the provided filename or fall back to the uploaded file's name
        var fileName = string.IsNullOrWhiteSpace(request.FileName)
            ? request.File.FileName
            : request.FileName;

        // Generate unique object name using ResourceId as the uploading progress ID
        var objectName = _fileStorageService.GenerateObjectName(
            LogoAppServiceName,
            fileName,
            request.ResourceId);

        // Determine content type
        var contentType = _fileStorageService.GetContentType(fileName);

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
}
