using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Persistence.Repositories.IdentityRepository;
public sealed class RoleRepository : RepositoryBase<Role, int>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<int> GetClientRoleIdAsync(RoleType roleType)
    {
        var roleEntity = await FindByIdAsync((int)roleType);
        return roleEntity?.Id ?? (int)RoleType.Client;
    }
}
