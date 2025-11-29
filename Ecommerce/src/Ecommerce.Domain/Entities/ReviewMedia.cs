using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities;

public class ReviewMedia : DomainEntity<int>
{
    public string Url { get; set; }
    public MediaType Type { get; set; }
    public int ReviewId { get; set; }

    // Navigation properties
    public virtual Review Review { get; set; }
}
