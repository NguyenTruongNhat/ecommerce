using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

/// <summary>
/// Handler for aborting a multipart upload to S3
/// This cancels the upload session and removes all uploaded parts to free up storage
/// </summary>
public sealed class AbortMultipartUploadCommandHandler
    : ICommandHandler<Command.AbortMultipartUploadCommand, Response.AbortMultipartUploadResponseDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<AbortMultipartUploadCommandHandler> _logger;

    public AbortMultipartUploadCommandHandler(
        IFileStorageService fileStorageService,
        ILogger<AbortMultipartUploadCommandHandler> logger)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Response.AbortMultipartUploadResponseDto>> Handle(
        Command.AbortMultipartUploadCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Aborting multipart upload. UploadId: {UploadId}, ObjectName: {ObjectName}", request.UploadId, request.ObjectName);

        // Abort the multipart upload in S3
        // This will remove all uploaded parts and free up storage
        await _fileStorageService.AbortMultipartUploadAsync(
            request.ObjectName,
            request.UploadId,
            cancellationToken);

        var response = new Response.AbortMultipartUploadResponseDto
        {
            ObjectName = request.ObjectName,
            UploadId = request.UploadId,
            Message = "Multipart upload aborted successfully. All uploaded parts have been removed.",
            AbortedAt = DateTime.UtcNow
        };

        return Result<Response.AbortMultipartUploadResponseDto>.Success(response);
    }
}
