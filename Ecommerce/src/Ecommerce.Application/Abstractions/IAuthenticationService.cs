using Ecommerce.Contract.Services.V1.Identity.Models;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Application.Abstractions;
public interface IAuthenticationService
{
    TokenResponse GenerateAccessAndRefreshTokens(User user, Device newDevice, string email);
    void SaveRefreshToken(string refreshToken, int deviceId);

}
