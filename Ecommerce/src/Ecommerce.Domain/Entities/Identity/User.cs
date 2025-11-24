using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities.Identity;

public class User : DomainEntity<int>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string Avatar { get; set; }
    public string TotpSecret { get; set; }
    public UserStatus Status { get; set; }
    public int RoleId { get; set; }
    public virtual Role Role { get; set; }
    public virtual ICollection<Device> Devices { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<CartItem> Carts { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
    public virtual ICollection<Message> SentMessages { get; set; }
    public virtual ICollection<Message> ReceivedMessages { get; set; }
}
