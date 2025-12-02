using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Enumerations;

namespace Ecommerce.Contract.Services.V1.Identity;

public static class Command
{
    public record Revoke(string AccessToken) : ICommand;
    public record SendOTPCommand(string Email, VerificationCodeType Type) : ICommand;
    public record RegisterCommand(
    string Email,
    string Password,
    string Name,
    string PhoneNumber,
    string ConfirmPassword,
    string Code) : ICommand;

    public record RefreshToken(string RefreshTokenValue) : ICommand;
    public record Logout(string RefreshTokenValue) : ICommand;
    public record ForgotPassword(
    string Email,
    string Code,
    string NewPassword,
    string ConfirmNewPassword) : ICommand;
    public record DisableTwoFactor(
    string TotpCode,
    string Code) : ICommand;
}
