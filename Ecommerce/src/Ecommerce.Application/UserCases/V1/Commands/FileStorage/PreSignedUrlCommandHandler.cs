using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public sealed class PreSignedUrlCommandHandler
    : ICommandHandler<Command.PreSignedUrlCommand, Response.PreSignedUrlResponseDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<PreSignedUrlCommandHandler> _logger;
    private const int DefaultExpirationMinutes = 60;

    public PreSignedUrlCommandHandler(
        IFileStorageService fileStorageService,
        ILogger<PreSignedUrlCommandHandler> logger)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Response.PreSignedUrlResponseDto>> Handle(
        Command.PreSignedUrlCommand request,
        CancellationToken cancellationToken)
    {
        // Generate presigned URL for download (expires in 60 minutes by default)
        var signedUrl = await _fileStorageService.GeneratePresignedDownloadUrlAsync(
            request.ObjectName,
            expiresInMinutes: DefaultExpirationMinutes,
            cancellationToken);

        var expiresAt = DateTime.UtcNow.AddMinutes(DefaultExpirationMinutes);

        var response = new Response.PreSignedUrlResponseDto
        {
            SignedUrl = signedUrl,
            ObjectName = request.ObjectName,
            FileName = request.FileName ?? Path.GetFileName(request.ObjectName),
            ExpiresInMinutes = DefaultExpirationMinutes,
            ExpiresAt = expiresAt
        };

        _logger.LogInformation(
            "Successfully generated presigned download URL for object: {ObjectName}, expires at: {ExpiresAt}",
            request.ObjectName, expiresAt);

        return Result<Response.PreSignedUrlResponseDto>.Success(response);
    }
}
