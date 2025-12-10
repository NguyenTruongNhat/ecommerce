using FluentValidation;

namespace Ecommerce.Contract.Services.V1.FileStorage.Validators;

public class AbortMultipartUploadValidator : AbstractValidator<Command.AbortMultipartUploadCommand>
{
    public AbortMultipartUploadValidator()
    {
        RuleFor(x => x.UploadId)
            .NotEmpty()
            .WithMessage("UploadId is required")
            .WithErrorCode("FileStorage.InvalidRequest");

        RuleFor(x => x.ObjectName)
            .NotEmpty()
            .WithMessage("ObjectName is required")
            .WithErrorCode("FileStorage.InvalidRequest");
    }
}
