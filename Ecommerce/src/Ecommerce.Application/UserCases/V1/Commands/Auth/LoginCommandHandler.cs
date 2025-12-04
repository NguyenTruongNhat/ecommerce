using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Exceptions;

namespace Ecommerce.Application.UserCases.V1.Commands.Auth;
public class LoginCommandHandler : ICommandHandler<Command.LoginCommand, Response.Authenticated>
{
    private readonly IUserRepository _userRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IHashingService _hashingService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAuthenticationService _authenticationService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IHashingService hashingService,
        IJwtTokenService jwtTokenService,
        IDeviceRepository deviceRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _hashingService = hashingService;
        _jwtTokenService = jwtTokenService;
        _deviceRepository = deviceRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _authenticationService = authenticationService;
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
        var tokens = _authenticationService.GenerateAccessAndRefreshTokens(user, newDevice, request.Email);

        // Store Refresh Token
        _authenticationService.SaveRefreshToken(tokens.RefreshToken, newDevice.Id);

        var response = new Response.Authenticated()
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        };
        return Result<Response.Authenticated>.Success(response);
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

}
