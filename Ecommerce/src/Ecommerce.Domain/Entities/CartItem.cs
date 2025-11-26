using Ecommerce.Domain.Abstractions.Entities;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Entities;

public class CartItem : DomainEntity<Guid>
{
    public int Quantity { get; set; }
    public int SkuId { get; set; }
    public int UserId { get; set; }

    // Navigation properties
    public virtual SKU Sku { get; set; }
    public virtual User User { get; set; }
}
