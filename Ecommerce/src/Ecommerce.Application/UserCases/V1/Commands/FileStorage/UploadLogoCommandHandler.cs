using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Constants;
using Ecommerce.Contract.Helper;
using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

/// <summary>
/// Handler for uploading logo files directly to S3 storage
/// This performs server-side upload, unlike signed URL approach where client uploads directly
/// </summary>
public sealed class UploadLogoCommandHandler
    : ICommandHandler<Command.UploadLogoCommand, Response.UploadLogoResponseDto>
{
    private readonly IFileStorageService _fileStorageService;

    public UploadLogoCommandHandler(
        IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
    }

    public async Task<Result<Response.UploadLogoResponseDto>> Handle(
        Command.UploadLogoCommand request,
        CancellationToken cancellationToken)
    {

        bool isImageFile = FileStorageFunctionHelper.IsImageFile(request.File.FileName);
        if (!isImageFile)
        {
            throw new ArgumentException("The uploaded file is not a valid image.");
        }
        string objectName = FileStorageFunctionHelper.CreateResourceObjectName("AccountId", FileStorageVariables.LogoFile, request.ResourceId, request.File.FileName);

        await _fileStorageService.UploadFileAsync(
            request.File,
            objectName,
            request.File.ContentType,
            cancellationToken);

        var response = new Response.UploadLogoResponseDto
        {
            ResourceId = request.ResourceId,
            ObjectName = objectName,
            UploadedAt = DateTime.UtcNow
        };

        return Result<Response.UploadLogoResponseDto>.Success(response);
    }
}
