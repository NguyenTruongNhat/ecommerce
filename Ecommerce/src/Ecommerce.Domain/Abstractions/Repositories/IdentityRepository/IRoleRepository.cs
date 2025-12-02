using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
public interface IRoleRepository : IRepositoryBase<Role, int>
{
    Task<int> GetClientRoleIdAsync(RoleType roloType);
}
