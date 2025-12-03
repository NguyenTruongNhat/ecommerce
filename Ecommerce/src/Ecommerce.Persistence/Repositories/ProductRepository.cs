using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Persistence.Repositories;
public sealed class ProductRepository : RepositoryBase<Product, Guid>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }
}


