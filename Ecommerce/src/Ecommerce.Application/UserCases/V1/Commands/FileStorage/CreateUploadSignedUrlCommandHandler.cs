using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public sealed class CreateUploadSignedUrlCommandHandler
    : ICommandHandler<Command.CreateUploadSignedUrlCommand, Response.CreateUploadSignedUrlResponseDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<CreateUploadSignedUrlCommandHandler> _logger;

    public CreateUploadSignedUrlCommandHandler(
        IFileStorageService fileStorageService,
        ILogger<CreateUploadSignedUrlCommandHandler> logger)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Response.CreateUploadSignedUrlResponseDto>> Handle(
        Command.CreateUploadSignedUrlCommand request,
        CancellationToken cancellationToken)
    {

        _logger.LogInformation(
            "Generating upload signed URL for AppService: {AppServiceName}, FileName: {FileName}, ProgressId: {ProgressId}",
            request.AppServiceName, request.FileName, request.UploadingProgressId);

        // Generate unique object name with proper structure
        var objectName = _fileStorageService.GenerateObjectName(
            request.AppServiceName,
            request.FileName,
            request.UploadingProgressId);

        // Determine content type from file extension
        var contentType = _fileStorageService.GetContentType(request.FileName);

        // Generate presigned URL for upload (expires in 60 minutes)
        var signedUrl = await _fileStorageService.GeneratePresignedUploadUrlAsync(
            objectName,
            contentType,
            expiresInMinutes: 60,
            cancellationToken);

        var response = new Response.CreateUploadSignedUrlResponseDto
        {
            SignedUrl = signedUrl,
            ContentType = contentType,
            ObjectName = objectName,
            FileType = Path.GetExtension(request.FileName)
        };

        _logger.LogInformation(
            "Successfully generated upload signed URL for object: {ObjectName}",
            objectName);

        return Result<Response.CreateUploadSignedUrlResponseDto>.Success(response);

    }
}
