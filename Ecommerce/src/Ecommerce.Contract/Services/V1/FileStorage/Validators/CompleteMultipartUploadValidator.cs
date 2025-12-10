using FluentValidation;

namespace Ecommerce.Contract.Services.V1.FileStorage.Validators;

public class CompleteMultipartUploadValidator : AbstractValidator<Command.CompleteMultipartUploadCommand>
{
    public CompleteMultipartUploadValidator()
    {
        RuleFor(x => x.UploadId)
            .NotEmpty()
            .WithMessage("UploadId is required")
            .WithErrorCode("FileStorage.InvalidRequest");

        RuleFor(x => x.ObjectName)
            .NotEmpty()
            .WithMessage("ObjectName is required")
            .WithErrorCode("FileStorage.InvalidRequest");

        RuleFor(x => x.PartETags)
            .NotEmpty()
            .WithMessage("At least one part ETag is required")
            .WithErrorCode("FileStorage.InvalidRequest");

        RuleFor(x => x.PartETags)
            .Must(HaveSequentialPartNumbers)
            .When(x => x.PartETags != null && x.PartETags.Count > 0)
            .WithMessage("Part numbers must be sequential starting from 1")
            .WithErrorCode("FileStorage.InvalidPartNumbers");

        RuleFor(x => x.PartETags)
            .Must(HaveValidETags)
            .When(x => x.PartETags != null && x.PartETags.Count > 0)
            .WithMessage("All parts must have valid ETags")
            .WithErrorCode("FileStorage.InvalidPartETag");
    }

    private static bool HaveSequentialPartNumbers(List<Amazon.S3.Model.PartETag> partETags)
    {
        if (partETags == null || partETags.Count == 0)
            return true;

        var sortedParts = partETags.OrderBy(p => p.PartNumber).ToList();
        for (int i = 0; i < sortedParts.Count; i++)
        {
            if (sortedParts[i].PartNumber != i + 1)
                return false;
        }
        return true;
    }

    private static bool HaveValidETags(List<Amazon.S3.Model.PartETag> partETags)
    {
        if (partETags == null || partETags.Count == 0)
            return true;

        return partETags.All(part => !string.IsNullOrWhiteSpace(part.ETag));
    }
}
