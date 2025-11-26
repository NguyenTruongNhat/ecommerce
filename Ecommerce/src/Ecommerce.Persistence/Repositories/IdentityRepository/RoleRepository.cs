using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Persistence.Repositories.IdentityRepository;
public sealed class RoleRepository : RepositoryBase<Role, int>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context)
    {
    }
}
