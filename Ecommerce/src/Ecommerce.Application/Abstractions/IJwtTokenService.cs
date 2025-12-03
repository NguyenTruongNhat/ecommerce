using System.Security.Claims;

namespace Ecommerce.Application.Abstractions;

public interface IJwtTokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken(IEnumerable<Claim> claims);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
