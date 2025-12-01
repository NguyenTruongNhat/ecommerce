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
        int roleId = (int)roleType;
        var roleEntity = await FindByIdAsync(roleId);
        return roleEntity?.Id ?? 3;
    }
}
