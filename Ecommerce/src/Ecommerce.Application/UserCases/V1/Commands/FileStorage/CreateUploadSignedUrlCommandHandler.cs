using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Helper;
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
        var folderName = Path.GetFileNameWithoutExtension(request.FileName);
        var dateTime = DateTime.Now;

        // Create object name to upload file to Cloud
        string objectName = FileStorageFunctionHelper.CreateObjectName(dateTime, "AccountId", request.AppServiceName, folderName, request.FileName);
        string fileType = FileStorageFunctionHelper.GetFileExtension(request.FileName);

        // Determine content type from file extension
        var contentType = FileStorageFunctionHelper.GetUploadContentType(request.FileName);

        // Generate presigned URL for upload (expires in 60 minutes)
        var signedUrl = await _fileStorageService.GeneratePresignedUploadUrlAsync(
            objectName,
            contentType,
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
