using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class SkuRepository : RepositoryBase<SKU, int>, ISkuRepository
{
    public SkuRepository(ApplicationDbContext context) : base(context)
    {
    }
}
