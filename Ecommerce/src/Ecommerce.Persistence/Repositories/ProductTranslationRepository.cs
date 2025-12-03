using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class ProductTranslationRepository : RepositoryBase<ProductTranslation, int>, IProductTranslationRepository
{
    public ProductTranslationRepository(ApplicationDbContext context) : base(context)
    {
    }
}
