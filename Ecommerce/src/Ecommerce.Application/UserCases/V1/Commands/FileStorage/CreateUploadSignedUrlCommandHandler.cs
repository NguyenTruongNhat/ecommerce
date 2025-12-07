using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public class CreateUploadSignedUrlCommandHandler : ICommandHandler<Command.CreateUploadSignedUrlCommand, Response.CreateUploadSignedUrlResponseDto>
{
    public Task<Result<Response.CreateUploadSignedUrlResponseDto>> Handle(Command.CreateUploadSignedUrlCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"AppServiceName: {request.AppServiceName}, FileName: {request.FileName}, UploadingProgressId: {request.UploadingProgressId}");
        return Task.FromResult(Result<Response.CreateUploadSignedUrlResponseDto>.Success(new Response.CreateUploadSignedUrlResponseDto()));
    }
}
