using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public class AbortMultipartUploadCommandHandler : ICommandHandler<Command.AbortMultipartUploadCommand>
{
    public Task<Result> Handle(Command.AbortMultipartUploadCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"UploadId: {request.UploadId}, ObjectName: {request.ObjectName}");
        return Task.FromResult(Result.Success());
    }
}
