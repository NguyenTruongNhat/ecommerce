using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Exceptions;
using static Ecommerce.Contract.Services.V1.Identity.Command;

namespace Ecommerce.Application.UserCases.V1.Commands.Auth;
public class LogoutCommandHandler : ICommandHandler<Command.LogoutCommand>
{
    private readonly IJwtTokenService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IDeviceRepository _deviceRepository;

    public LogoutCommandHandler(
        IJwtTokenService jwtService,
        IRefreshTokenRepository refreshTokenRepository,
        IDeviceRepository deviceRepository)
    {
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
        _deviceRepository = deviceRepository;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var verificationResult = _jwtService.GetPrincipalFromExpiredToken(request.RefreshToken) 
            ?? throw new CommonException.InvalidToken();

        var refreshTokenEntity = await _refreshTokenRepository.FindSingleAsync(
            x => x.Token == request.RefreshToken,
            cancellationToken
        ) ?? throw new CommonException.NotFound();

        var deviceId = refreshTokenEntity.DeviceId;

        _refreshTokenRepository.Remove(refreshTokenEntity);

        var device = await _deviceRepository.FindSingleAsync(d => d.Id == deviceId, cancellationToken);

        if (device is null)
        {
            // If the Device is not found, still continue deleting the token and return success.
            // (However, in a strict system, this could be considered a NotFound error)
        }
        else
        {
            device.IsActive = false;
            _deviceRepository.Update(device);
        }

        return Result.Success();
    }
}
