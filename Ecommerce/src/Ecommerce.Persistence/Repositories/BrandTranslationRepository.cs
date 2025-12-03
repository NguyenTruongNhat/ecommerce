using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class BrandTranslationRepository : RepositoryBase<BrandTranslation, int>, IBrandTranslationRepository
{
    public BrandTranslationRepository(ApplicationDbContext context) : base(context)
    {
    }
}
