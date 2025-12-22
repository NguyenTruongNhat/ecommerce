using System;
using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Helper;
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
        var folderName = Path.GetFileNameWithoutExtension(request.FileName);
        var dateTime = DateTime.Now;

        // Create object name to upload file to Cloud
        string objectName = FileStorageFunctionHelper.CreateObjectName(dateTime, "AccountId", request.AppServiceName, folderName, request.FileName);
        string fileType = FileStorageFunctionHelper.GetFileExtension(request.FileName);

        // Initiate multipart upload in S3
        var uploadId = await _fileStorageService.InitiateMultipartUploadAsync(objectName);

        var response = new Response.InitiateMultipartUploadResponseDto
        {
            UploadId = uploadId,
            ObjectName = objectName,
            FileType = fileType,
        };

        return Result<Response.InitiateMultipartUploadResponseDto>.Success(response);
    }
}
