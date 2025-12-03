using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class ProductSKUSnapshotRepository : RepositoryBase<ProductSKUSnapshot, int>, IProductSKUSnapshotRepository
{
    public ProductSKUSnapshotRepository(ApplicationDbContext context) : base(context)
    {
    }
}
