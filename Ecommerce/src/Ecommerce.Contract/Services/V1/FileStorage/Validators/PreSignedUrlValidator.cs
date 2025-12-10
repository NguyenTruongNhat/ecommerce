using FluentValidation;

namespace Ecommerce.Contract.Services.V1.FileStorage.Validators;

public class PreSignedUrlValidator : AbstractValidator<Command.PreSignedUrlCommand>
{
    public PreSignedUrlValidator()
    {
        RuleFor(x => x.ObjectName)
            .NotEmpty()
            .WithMessage("ObjectName is required")
            .WithErrorCode("FileStorage.InvalidRequest");
    }
}
