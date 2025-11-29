using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities;

public class Payment : DomainEntity<int>
{ 
    public PaymentStatus Status { get; set; }

    // Navigation properties
    public virtual ICollection<Order> Orders { get; set; }
}
