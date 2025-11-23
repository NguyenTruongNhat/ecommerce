using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int SkuId { get; set; }
    public int UserId { get; set; }

    // Navigation properties
    public virtual SKU Sku { get; set; }
    public virtual AppUser User { get; set; }
}
