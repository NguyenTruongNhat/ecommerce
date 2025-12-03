using System.Security.Claims;
using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Enumerations;
using Ecommerce.Contract.Security;
using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Exceptions;
using Ecommerce.Persistence;

namespace Ecommerce.Application.UserCases.V1.Commands.Auth;
public class LoginCommandHandler : ICommandHandler<Command.LoginCommand, Response.Authenticated>
{
    private readonly IUserRepository _userRepository;
    private readonly IHashingService _hashingService;
    private readonly IJwtTokenService _jwtTokenService;

    private readonly ApplicationDbContext _context;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IHashingService hashingService,
        ApplicationDbContext context,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _hashingService = hashingService;
        //_twoFactorService = twoFactorService;
        //_verificationService = verificationService;
        //_tokenService = tokenService;
        _context = context;
        _jwtTokenService = jwtTokenService;
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

        var newDevice = new Device
        {
            //UserId = user.Id,
            //UserAgent = request.UserAgent,
            //Ip = request.Ip,
        };

        // _deviceRepository.Add(newDevice);
        _context.Set<Device>().Add(newDevice); // Hoặc thêm trực tiếp qua Context

        // Generate JWT Token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimKeys.DeviceId, newDevice.Id.ToString()),
            new Claim(ClaimKeys.RoleId, user.RoleId.ToString())
        };
        var claimsForRefreshToken = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var accessToken = _jwtTokenService.GenerateAccessToken(claims);
        var refreshToken = _jwtTokenService.GenerateRefreshToken(claimsForRefreshToken);

        var response = new Response.Authenticated()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        return Result<Response.Authenticated>.Success(response);
    }
}
