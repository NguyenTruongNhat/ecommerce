using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Security;
using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Contract.Services.V1.Identity.Models;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Entities.Identity;
using Ecommerce.Domain.Exceptions;

namespace Ecommerce.Application.UserCases.V1.Commands.Auth;
public class LoginCommandHandler : ICommandHandler<Command.LoginCommand, Response.Authenticated>
{
    private readonly IUserRepository _userRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IHashingService _hashingService;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IHashingService hashingService,
        IJwtTokenService jwtTokenService,
        IDeviceRepository deviceRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _hashingService = hashingService;
        _jwtTokenService = jwtTokenService;
        _deviceRepository = deviceRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result<Response.Authenticated>> Handle(Command.LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindSingleAsync(
            x => x.Email == request.Email,
            cancellationToken,
            x => x.Role // Include Role
        ) ?? throw new CommonException.NotFound();

        var isPasswordMatch = _hashingService.Compare(request.Password, user.Password);
        if (!isPasswordMatch)
            throw new CommonException.WrongPassword();

        // TODO: Check 2FA if needed

        var newDevice = await CreateDeviceAsync(user.Id, request);

        // Generate JWT Token
        var tokens = GenerateTokens(user, newDevice, request);

        // Store Refresh Token
        SaveRefreshToken(tokens.RefreshToken, newDevice.Id);

        var response = new Response.Authenticated()
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        };
        return Result<Response.Authenticated>.Success(response);
    }

    private void SaveRefreshToken(string refreshToken, int id)
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
            DeviceId = id
        });
    }

    private async Task<Device> CreateDeviceAsync(
    int userId,
    Command.LoginCommand request)
    {
        var newDevice = new Device
        {
            UserId = userId,
            UserAgent = request.UserAgent,
            Ip = request.Ip,
            LastActive = DateTime.UtcNow,
            IsActive = true
        };

        _deviceRepository.Add(newDevice);

        return newDevice;
    }

    private TokenResponse GenerateTokens(User user, Device newDevice, Command.LoginCommand request)
    {
        var accessTokenClaims = new List<Claim>{
        new Claim(ClaimTypes.Email, request.Email),
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
}
