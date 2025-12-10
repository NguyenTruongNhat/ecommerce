using FluentValidation;

namespace Ecommerce.Contract.Services.V1.FileStorage.Validators;

public class CreateUploadSignedUrlValidator : AbstractValidator<Command.CreateUploadSignedUrlCommand>
{
    public CreateUploadSignedUrlValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("FileName is required")
            .WithErrorCode("FileStorage.InvalidRequest");

        RuleFor(x => x.UploadingProgressId)
            .NotEmpty()
            .WithMessage("UploadingProgressId is required")
            .WithErrorCode("FileStorage.InvalidRequest");
    }
}
