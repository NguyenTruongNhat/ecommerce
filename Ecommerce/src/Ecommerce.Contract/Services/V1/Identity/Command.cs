using Ecommerce.Contract.Abstractions.Message;

namespace Ecommerce.Contract.Services.V1.Identity;

public static class Command
{
    public record Revoke(string AccessToken) : ICommand;
}
