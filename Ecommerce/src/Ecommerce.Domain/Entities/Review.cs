using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
    public int OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int UserId { get; set; }
    public int UpdateCount { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; }
    public virtual Product Product { get; set; }
    public virtual User User { get; set; }
    public virtual ICollection<ReviewMedia> Medias { get; set; }
}
