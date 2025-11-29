using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities;

public class SKU : DomainEntity<int>
{
    public string Value { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
    public string Image { get; set; }
    public Guid ProductId { get; set; }

    public int CreatedById { get; set; }
    public int? UpdatedById { get; set; }
    public int? DeletedById { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; }
    public virtual ICollection<ProductSKUSnapshot> ProductSKUSnapshots { get; set; }
}
