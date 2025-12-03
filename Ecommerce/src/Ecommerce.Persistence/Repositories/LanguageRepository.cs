using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class LanguageRepository : RepositoryBase<Language, int>, ILanguageRepository
{
    public LanguageRepository(ApplicationDbContext context) : base(context)
    {
    }
}
