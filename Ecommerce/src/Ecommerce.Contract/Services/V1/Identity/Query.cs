using Ecommerce.Contract.Abstractions.Message;

namespace Ecommerce.Contract.Services.V1.Identity;

public static class Query
{
    public record Token(string? AccessToken, string? RefreshToken) : IQuery<Response.Authenticated>;
}
