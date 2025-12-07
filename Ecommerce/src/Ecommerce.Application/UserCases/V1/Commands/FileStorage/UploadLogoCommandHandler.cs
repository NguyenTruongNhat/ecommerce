using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public class UploadLogoCommandHandler : ICommandHandler<Command.UploadLogoCommand>
{
    public Task<Result> Handle(Command.UploadLogoCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"ResourceId: {request.ResourceId}, FileName: {request.FileName}, File: {request.File?.FileName}");
        return Task.FromResult(Result.Success());
    }
}
