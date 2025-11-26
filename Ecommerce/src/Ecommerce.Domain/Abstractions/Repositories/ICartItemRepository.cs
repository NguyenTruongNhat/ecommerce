using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Abstractions.Repositories;
public interface ICartItemRepository : IRepositoryBase<CartItem, int>
{
    // Add cart-item specific methods if needed
}
