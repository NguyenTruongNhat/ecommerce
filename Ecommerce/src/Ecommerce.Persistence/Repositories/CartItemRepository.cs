using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Entities;
using Ecommerce.Persistence;

namespace Ecommerce.Persistence.Repositories;
public sealed class CartItemRepository : RepositoryBase<CartItem, int>, ICartItemRepository
{
    public CartItemRepository(ApplicationDbContext context) : base(context)
    {
    }
}
