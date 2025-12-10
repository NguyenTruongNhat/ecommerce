using FluentValidation;

namespace Ecommerce.Contract.Services.V1.FileStorage.Validators;

public class CreateMultipartUploadSignedUrlValidator : AbstractValidator<Command.CreateMultipartUploadSignedUrlCommand>
{
    private const int MaxPartNumber = 10000;

    public CreateMultipartUploadSignedUrlValidator()
    {
        RuleFor(x => x.UploadId)
            .NotEmpty()
            .WithMessage("UploadId is required")
            .WithErrorCode("FileStorage.InvalidRequest");

        RuleFor(x => x.ObjectName)
            .NotEmpty()
            .WithMessage("ObjectName is required")
            .WithErrorCode("FileStorage.InvalidRequest");

        RuleFor(x => x.PartNumber)
            .InclusiveBetween(1, MaxPartNumber)
            .WithMessage($"PartNumber must be between 1 and {MaxPartNumber}")
            .WithErrorCode("FileStorage.InvalidPartNumber");
    }
}
