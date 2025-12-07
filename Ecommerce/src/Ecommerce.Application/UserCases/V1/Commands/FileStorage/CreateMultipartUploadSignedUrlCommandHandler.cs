using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;
using static Ecommerce.Contract.Services.V1.FileStorage.Response;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public class CreateMultipartUploadSignedUrlCommandHandler : ICommandHandler<Command.CreateMultipartUploadSignedUrlCommand, Response.CreateMultipartUploadResponseDto>
{
    public Task<Result<Response.CreateMultipartUploadResponseDto>> Handle(Command.CreateMultipartUploadSignedUrlCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"AppServiceName: {request.AppServiceName}, FileName: {request.FileName}, UploadingProgressId: {request.UploadingProgressId}, PartNumber: {request.PartNumber}, UploadId: {request.UploadId}, ObjectName: {request.ObjectName}");
        return Task.FromResult(Result<Response.CreateMultipartUploadResponseDto>.Success(new CreateMultipartUploadResponseDto()));
    }
}
