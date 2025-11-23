using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Entities;

public class Device
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserAgent { get; set; }
    public string Ip { get; set; }
    public DateTime LastActive { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public virtual AppUser User { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
}
