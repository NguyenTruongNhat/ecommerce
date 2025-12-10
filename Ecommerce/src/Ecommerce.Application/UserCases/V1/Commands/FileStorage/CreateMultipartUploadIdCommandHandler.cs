using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

/// <summary>
/// Handler for initiating a multipart upload to S3
/// This creates an upload session and returns an upload ID that will be used for all subsequent part uploads
/// </summary>
public sealed class CreateMultipartUploadIdCommandHandler
    : ICommandHandler<Command.CreateMultipartUploadCommand, Response.InitiateMultipartUploadResponseDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<CreateMultipartUploadIdCommandHandler> _logger;

    public CreateMultipartUploadIdCommandHandler(
        IFileStorageService fileStorageService,
        ILogger<CreateMultipartUploadIdCommandHandler> logger)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Response.InitiateMultipartUploadResponseDto>> Handle(
        Command.CreateMultipartUploadCommand request,
        CancellationToken cancellationToken)
    {
        // Generate unique object name
        var objectName = _fileStorageService.GenerateObjectName(
            request.AppServiceName,
            request.FileName,
            request.UploadingProgressId);

        // Determine content type from file extension
        var contentType = _fileStorageService.GetContentType(request.FileName);
        var fileType = Path.GetExtension(request.FileName);


        // Initiate multipart upload in S3
        var uploadId = await _fileStorageService.InitiateMultipartUploadAsync(
            objectName,
            contentType,
            cancellationToken);

        var response = new Response.InitiateMultipartUploadResponseDto
        {
            UploadId = uploadId,
            ObjectName = objectName,
            FileName = request.FileName,
            ContentType = contentType,
            FileType = fileType,
            InitiatedAt = DateTime.UtcNow
        };
                _logger.LogInformation(            "Multipart upload initiated successfully. UploadId: {UploadId}, ObjectName: {ObjectName}",            uploadId, objectName);

        return Result<Response.InitiateMultipartUploadResponseDto>.Success(response);
    }
}
