using FluentValidation;

namespace Ecommerce.Contract.Services.V1.Identity.Validators;


public class SendOTPValidator : AbstractValidator<Command.SendOTPCommand>
{
    public SendOTPValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
