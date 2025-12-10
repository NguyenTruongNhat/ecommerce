using FluentValidation;

namespace Ecommerce.Contract.Services.V1.FileStorage.Validators;

public class UploadMultipleFilesValidator : AbstractValidator<Command.UploadMultipleFilesCommand>
{
    private const long MaxFileSizeInBytes = 100 * 1024 * 1024; // 100MB per file
    private const int MaxFileCount = 20;

    public UploadMultipleFilesValidator()
    {
        RuleFor(x => x.Files)
            .NotEmpty()
            .WithMessage("At least one file must be provided")
            .WithErrorCode("FileStorage.NoFiles");

        RuleFor(x => x.Files)
            .Must(files => files == null || files.Count <= MaxFileCount)
            .WithMessage($"Maximum {MaxFileCount} files allowed per upload")
            .WithErrorCode("FileStorage.TooManyFiles");

        RuleFor(x => x.UploadingProgressId)
            .NotEmpty()
            .WithMessage("UploadingProgressId is required")
            .WithErrorCode("FileStorage.InvalidProgressId");

        RuleForEach(x => x.Files)
            .Must(ValidateIndividualFile)
            .When(x => x.Files != null)
            .WithMessage("One or more files are invalid")
            .WithErrorCode("FileStorage.InvalidFile");
    }

    private static bool ValidateIndividualFile(Microsoft.AspNetCore.Http.IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        if (file.Length > MaxFileSizeInBytes)
            return false;

        if (string.IsNullOrWhiteSpace(file.FileName))
            return false;

        return true;
    }
}
