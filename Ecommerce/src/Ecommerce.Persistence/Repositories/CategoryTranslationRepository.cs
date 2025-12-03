using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class CategoryTranslationRepository : RepositoryBase<CategoryTranslation, int>, ICategoryTranslationRepository
{
    public CategoryTranslationRepository(ApplicationDbContext context) : base(context)
    {
    }
}
