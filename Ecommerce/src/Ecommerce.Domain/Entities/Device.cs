using Ecommerce.Domain.Abstractions.Entities;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Entities;

public class Device : DomainEntity<Guid>
{
    public int UserId { get; set; }
    public string UserAgent { get; set; }
    public string Ip { get; set; }
    public DateTime LastActive { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
}
