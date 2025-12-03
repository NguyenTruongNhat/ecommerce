using FluentValidation;

namespace Ecommerce.Contract.Services.V1.Identity.Validators;

public class LoginValidator : AbstractValidator<Command.LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.UserAgent).NotEmpty();
        RuleFor(x => x.Ip).NotEmpty();
        RuleFor(x => x.TotpCode).MaximumLength(6).When(x => !string.IsNullOrEmpty(x.TotpCode));
        RuleFor(x => x.Code).MaximumLength(6).When(x => !string.IsNullOrEmpty(x.Code));

    }
}
