using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Abstractions.Entities;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Entities;

public class Order : DomainEntity<Guid>
{
    public int UserId { get; set; }
    public OrderStatus Status { get; set; }
    public string Receiver { get; set; }
    public int? ShopId { get; set; }
    public int PaymentId { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual User Shop { get; set; }
    public virtual Payment Payment { get; set; }
    public virtual ICollection<ProductSKUSnapshot> Items { get; set; }
    public virtual ICollection<Product> Products { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
}
