using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Abstractions.Repositories;
public interface IOrderRepository : IRepositoryBase<Order, int>
{
    // Add order-specific methods if needed
}
