namespace Ecommerce.Contract.Services.V1.Identity.Models;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
