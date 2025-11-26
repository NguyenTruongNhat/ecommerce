using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Persistence.Repositories.ProductRepository;
public sealed class ProductRepository : RepositoryBase<Product, Guid>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }
}


