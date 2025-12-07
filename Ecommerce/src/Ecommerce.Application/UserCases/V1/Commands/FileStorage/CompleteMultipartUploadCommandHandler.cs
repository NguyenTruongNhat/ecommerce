using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public class CompleteMultipartUploadCommandHandler : ICommandHandler<Command.CompleteMultipartUploadCommand>
{
    public Task<Result> Handle(Command.CompleteMultipartUploadCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"UploadId: {request.UploadId}, ObjectName: {request.ObjectName}, PartETagsCount: {request.PartETags?.Count}");
        return Task.FromResult(Result.Success());
    }
}
