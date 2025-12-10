using FluentValidation;

namespace Ecommerce.Contract.Services.V1.FileStorage.Validators;

public class UploadLogoValidator : AbstractValidator<Command.UploadLogoCommand>
{
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5MB
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".svg", ".webp"];

    public UploadLogoValidator()
    {
        RuleFor(x => x.ResourceId)
            .NotEmpty()
            .WithMessage("ResourceId is required")
            .WithErrorCode("FileStorage.InvalidRequest");

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required and cannot be empty")
            .WithErrorCode("FileStorage.InvalidFile");

        RuleFor(x => x.File)
            .Must(file => file != null && file.Length > 0)
            .When(x => x.File != null)
            .WithMessage("File cannot be empty")
            .WithErrorCode("FileStorage.InvalidFile");

        RuleFor(x => x.File)
            .Must(file => file == null || file.Length <= MaxFileSizeInBytes)
            .WithMessage($"File size exceeds maximum allowed size of {MaxFileSizeInBytes / 1024 / 1024}MB")
            .WithErrorCode("FileStorage.FileTooLarge");

        RuleFor(x => x.File)
            .Must(HasValidExtension)
            .When(x => x.File != null)
            .WithMessage($"Only image files are allowed. Supported formats: {string.Join(", ", AllowedExtensions)}")
            .WithErrorCode("FileStorage.InvalidFileType");

        RuleFor(x => x.File)
            .Must(HasValidContentType)
            .When(x => x.File != null)
            .WithMessage("File must be an image")
            .WithErrorCode("FileStorage.InvalidContentType");
    }

    private static bool HasValidExtension(Microsoft.AspNetCore.Http.IFormFile file)
    {
        if (file == null) return true;

        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        return !string.IsNullOrEmpty(extension) && AllowedExtensions.Contains(extension);
    }

    private static bool HasValidContentType(Microsoft.AspNetCore.Http.IFormFile file)
    {
        if (file == null) return true;

        return file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }
}
