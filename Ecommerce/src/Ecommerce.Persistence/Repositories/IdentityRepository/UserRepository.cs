using System.Linq.Expressions;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Persistence.Repositories.IdentityRepository;

public sealed class UserRepository : RepositoryBase<User, int>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }
}
