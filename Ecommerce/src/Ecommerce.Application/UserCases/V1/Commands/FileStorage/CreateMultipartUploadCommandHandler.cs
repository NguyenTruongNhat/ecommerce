using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public class CreateMultipartUploadCommandHandler : ICommandHandler<Command.CreateMultipartUploadCommand>
{
    public Task<Result> Handle(Command.CreateMultipartUploadCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"AppServiceName: {request.AppServiceName}, FileName: {request.FileName}, UploadingProgressId: {request.UploadingProgressId}");
        return Task.FromResult(Result.Success());
    }
}
