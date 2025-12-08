using Amazon.S3;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public class UploadAvatarCommandHandler : ICommandHandler<Command.UploadAvatarCommand, Response.UploadAvatarResponseDto>
{
    private readonly IAmazonS3 _amazonS3;
    public UploadAvatarCommandHandler(IAmazonS3 amazonS3)
    {
        _amazonS3 = amazonS3;
    }
    public Task<Result<Response.UploadAvatarResponseDto>> Handle(Command.UploadAvatarCommand request, CancellationToken cancellationToken)
    {

        Console.WriteLine($"ImageName: {request.ImageName}, File: {request.File?.FileName}");
        return Task.FromResult(Result<Response.UploadAvatarResponseDto>.Success(new Response.UploadAvatarResponseDto()));
    }
}
