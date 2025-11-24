using Ecommerce.Contract.Enumerations;

namespace Ecommerce.Domain.Entities;

public class ReviewMedia
{
    public int Id { get; set; }
    public string Url { get; set; }
    public MediaType Type { get; set; }
    public int ReviewId { get; set; }

    // Navigation properties
    public virtual Review Review { get; set; }
}
