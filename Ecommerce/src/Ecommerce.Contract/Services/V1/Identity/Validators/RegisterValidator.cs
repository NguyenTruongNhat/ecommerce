using FluentValidation;

namespace Ecommerce.Contract.Services.V1.Identity.Validators;

public class RegisterValidator : AbstractValidator<Command.RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(15);
        RuleFor(x => x.Code).NotEmpty().Length(6);
    }
}
