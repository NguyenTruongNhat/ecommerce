using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Persistence.Repositories;
public class RefreshTokenRepository : RepositoryBase<RefreshToken, int>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }
}

