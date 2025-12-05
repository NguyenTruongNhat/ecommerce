namespace Ecommerce.Contract.Services.V1.Identity;

public static class Response
{
    public class Authenticated
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }

    public class GoogleOAuthAuthenticated
    {
        public string? Link { get; set; }
    }
    public class AuthTokenResponse
    {
        public string URL { get; set; } = "";
    }

    public class GoogleCallbackResponse
    {
        public string Link { get; set; }
    }
}
