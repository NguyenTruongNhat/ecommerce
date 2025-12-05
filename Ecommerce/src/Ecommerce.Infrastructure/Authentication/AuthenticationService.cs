using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Security;
using Ecommerce.Contract.Services.V1.Identity.Models;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Infrastructure.Authentication;
public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    public AuthenticationService(IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository)
    {
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public TokenResponse GenerateAccessAndRefreshTokens(User user, Device newDevice, string email)
    {
        var accessTokenClaims = new List<Claim>{
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, user.Role.Name),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimKeys.DeviceId, newDevice.Id.ToString()),
        new Claim(ClaimKeys.RoleId, user.RoleId.ToString())};

        var refreshTokenClaims = new List<Claim>{
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())};

        var accessToken = _jwtTokenService.GenerateAccessToken(accessTokenClaims);
        var refreshToken = _jwtTokenService.GenerateRefreshToken(refreshTokenClaims);

        return new TokenResponse() { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public void SaveRefreshToken(string refreshToken, int deviceId)
    {
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(refreshToken);
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
        // Retrieve the expiration time (exp claim) from the claims
        var expClaim = (principal.FindFirst(JwtRegisteredClaimNames.Exp)?.Value) ?? throw new InvalidOperationException("The refresh token does not contain an expiration claim.");

        // Convert the expiration time from Unix timestamp to DateTime
        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;

        _refreshTokenRepository.Add(new RefreshToken
        {
            UserId = int.Parse(userId),
            Token = refreshToken,
            ExpiresAt = expiresAt,
            DeviceId = deviceId
        });
    }
}
