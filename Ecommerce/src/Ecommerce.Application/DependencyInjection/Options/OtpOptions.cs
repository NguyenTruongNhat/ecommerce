namespace Ecommerce.Application.DependencyInjection.Options;
public class OtpOptions
{
    public int ExpiresInMs { get; set; } = 5 * 60 * 1000;
}
