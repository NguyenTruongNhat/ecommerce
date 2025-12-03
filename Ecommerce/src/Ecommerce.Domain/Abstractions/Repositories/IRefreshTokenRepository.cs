using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Abstractions.Repositories;
public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken, int>
{
    // Assumes Token (string) is the key; adjust if your model differs.
}
