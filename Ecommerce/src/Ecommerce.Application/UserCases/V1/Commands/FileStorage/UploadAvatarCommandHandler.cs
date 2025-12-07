using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public class UploadAvatarCommandHandler : ICommandHandler<Command.UploadAvatarCommand, Response.UploadAvatarResponseDto>
{
    public Task<Result<Response.UploadAvatarResponseDto>> Handle(Command.UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"ImageName: {request.ImageName}, File: {request.File?.FileName}");
        return Task.FromResult(Result<Response.UploadAvatarResponseDto>.Success(new Response.UploadAvatarResponseDto()));
    }
}
