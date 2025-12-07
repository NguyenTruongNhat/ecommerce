using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

public class UploadMultipleFilesCommandHandler : ICommandHandler<Command.UploadMultipleFilesCommand>
{
    public Task<Result> Handle(Command.UploadMultipleFilesCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"AppServiceName: {request.AppServiceName}, FileName: {request.FileName}, UploadingProgressId: {request.UploadingProgressId}, FilesCount: {request.Files?.Count}");
        return Task.FromResult(Result.Success());
    }
}
