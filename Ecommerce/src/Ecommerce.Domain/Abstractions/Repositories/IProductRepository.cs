using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Abstractions.Repositories;
public interface IProductRepository : IRepositoryBase<Product, Guid>
{
    // Declare product-specific repository methods here (if any).
}
