using System.Security.Claims;
using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Entities.Identity;
using Ecommerce.Domain.Exceptions;

namespace Ecommerce.Application.UserCases.V1.Commands.Auth;
public class RefreshTokenCommandHandler : ICommandHandler<Command.RefreshTokenCommand, Response.Authenticated>
{

    private readonly IRoleRepository _roleRepository;
    private readonly IHashingService _hashingService;
    private readonly IUserRepository _userRepository;
    private readonly IVerificationCodeRepository _verificationCodeRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IAuthenticationService _authenticationService;

    public RefreshTokenCommandHandler(
        IRoleRepository roleRepository,
        IHashingService hashingService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IVerificationCodeRepository verificationCodeRepository,
        IJwtTokenService jwtTokenService,
        IDeviceRepository deviceRepository,
        IAuthenticationService authenticationService)
    {
        _roleRepository = roleRepository;
        _hashingService = hashingService;
        _userRepository = userRepository;
        _verificationCodeRepository = verificationCodeRepository;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _deviceRepository = deviceRepository;
        _authenticationService = authenticationService;
    }

    public async Task<Result<Response.Authenticated>> Handle(Command.RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Check whether the refreshToken is valid
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.RefreshTokenValue);
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier).ToString() ?? throw new CommonException.InvalidToken();

        // 2. Check whether the refreshToken exists in the database
        //    If the token has already been refreshed, notify the user
        //    that their refresh token may have been stolen
        var refreshToken = await _refreshTokenRepository.FindSingleAsync(x => x.Token == request.RefreshTokenValue, cancellationToken,
                                                                              includeProperties: o => o.User)
                                                                           ?? throw new CommonException.InvalidToken();

        // 3. Update the device
        var device = await UpdateDeviceAsync(refreshToken.DeviceId, request.Ip, request.UserAgent, cancellationToken);

        // 4. Delete the old refreshToken
        _refreshTokenRepository.Remove(refreshToken);

        // 5. Create a new accessToken and refreshToken
        var tokens = _authenticationService.GenerateAccessAndRefreshTokens(refreshToken.User, device, refreshToken.User.Email);

        // Store Refresh Token
        _authenticationService.SaveRefreshToken(tokens.RefreshToken, device.Id);

        var response = new Response.Authenticated()
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        };
        return Result<Response.Authenticated>.Success(response);
    }

    private async Task<Device> UpdateDeviceAsync(int deviceId, string ip, string userAgent, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.FindByIdAsync(deviceId, cancellationToken) ?? throw new CommonException.NotFound();
        device.Ip = ip;
        device.UserAgent = userAgent;
        device.LastActive = DateTime.UtcNow;
        _deviceRepository.Update(device);
        return device;
    }
}
