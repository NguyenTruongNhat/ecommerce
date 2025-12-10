using FluentValidation;

namespace Ecommerce.Contract.Services.V1.FileStorage.Validators;

public class CreateMultipartUploadValidator : AbstractValidator<Command.CreateMultipartUploadCommand>
{
    public CreateMultipartUploadValidator()
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
