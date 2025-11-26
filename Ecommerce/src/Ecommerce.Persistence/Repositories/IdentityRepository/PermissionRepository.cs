using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Persistence.Repositories.IdentityRepository;

public sealed class PermissionRepository : RepositoryBase<Permission, int>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context) : base(context)
    {
    }
}
