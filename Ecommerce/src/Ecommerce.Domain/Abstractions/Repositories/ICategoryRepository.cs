using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Abstractions.Repositories;
public interface ICategoryRepository : IRepositoryBase<Category, int>
{
    // Add category-specific methods if needed
}
