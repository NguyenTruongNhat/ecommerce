using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class UserTranslationRepository : RepositoryBase<UserTranslation, int>, IUserTranslationRepository
{
    public UserTranslationRepository(ApplicationDbContext context) : base(context)
    {
    }
}
