using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public class PreSignedUrlCommandHandler : ICommandHandler<Command.PreSignedUrlCommand>
{
    public Task<Result> Handle(Command.PreSignedUrlCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"ObjectName: {request.ObjectName}, FileName: {request.FileName}");
        return Task.FromResult(Result.Success());
    }
}
