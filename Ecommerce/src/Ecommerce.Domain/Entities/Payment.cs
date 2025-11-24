using Ecommerce.Contract.Enumerations;

namespace Ecommerce.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public PaymentStatus Status { get; set; }

    // Navigation properties
    public virtual ICollection<Order> Orders { get; set; }
}
