using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

/// <summary>
/// Handler for completing a multipart upload to S3
/// This combines all uploaded parts into a single object and finalizes the upload session
/// </summary>
public sealed class CompleteMultipartUploadCommandHandler
    : ICommandHandler<Command.CompleteMultipartUploadCommand, Response.CompleteMultipartUploadResponseDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<CompleteMultipartUploadCommandHandler> _logger;

    public CompleteMultipartUploadCommandHandler(
        IFileStorageService fileStorageService,
        ILogger<CompleteMultipartUploadCommandHandler> logger)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Response.CompleteMultipartUploadResponseDto>> Handle(
        Command.CompleteMultipartUploadCommand request,
        CancellationToken cancellationToken)
    {

        // Complete the multipart upload in S3
        var s3Response = await _fileStorageService.CompleteMultipartUploadAsync(
            request.ObjectName,
            request.UploadId,
            request.PartETags,
            cancellationToken);

        var response = new Response.CompleteMultipartUploadResponseDto
        {
            ObjectName = request.ObjectName,
            UploadId = request.UploadId,
            ETag = s3Response.ETag,
            Location = s3Response.Location,
            TotalParts = request.PartETags.Count,
            CompletedAt = DateTime.UtcNow
        };

        return Result<Response.CompleteMultipartUploadResponseDto>.Success(response);
    }
}
