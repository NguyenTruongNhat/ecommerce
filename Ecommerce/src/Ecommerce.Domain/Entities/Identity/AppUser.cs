using Ecommerce.Contract.Enumerations;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Domain.Entities.Identity;

public class AppUser : IdentityUser<Guid>
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string Avatar { get; set; }
    public string TotpSecret { get; set; }
    public UserStatus Status { get; set; }
    public Guid RoleId { get; set; }
    public virtual AppRole Role { get; set; }

    public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }
    public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }
    public virtual ICollection<IdentityUserToken<Guid>> Tokens { get; set; }
    public virtual ICollection<Device> Devices { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<CartItem> Carts { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
}
