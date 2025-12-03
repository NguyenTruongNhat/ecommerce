using FluentValidation;

namespace Ecommerce.Contract.Services.V1.Identity.Validators;

public class LoginValidator : AbstractValidator<Command.LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
