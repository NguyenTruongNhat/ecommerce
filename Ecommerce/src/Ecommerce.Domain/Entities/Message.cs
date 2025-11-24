using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Entities;

public class Message
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public string Content { get; set; }
    public DateTime? ReadAt { get; set; }

    // Navigation properties
    public virtual User FromUser { get; set; }
    public virtual User ToUser { get; set; }
}
