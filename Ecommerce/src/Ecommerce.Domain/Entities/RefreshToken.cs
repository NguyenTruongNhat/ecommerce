using Ecommerce.Domain.Abstractions.Entities;
using Ecommerce.Domain.Entities.Identity;

namespace Ecommerce.Domain.Entities;

public class RefreshToken : DomainEntity<int>
{
    public string Token { get; set; }
    public int UserId { get; set; }
    public int DeviceId { get; set; }
    public DateTime ExpiresAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual Device Device { get; set; }
}
