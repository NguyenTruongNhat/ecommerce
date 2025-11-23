using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Domain.Entities.Identity;

public class AppRole : IdentityRole<Guid>
{
    public string Description { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<IdentityRoleClaim<Guid>> Claims { get; set; }
    public virtual ICollection<AppPermission> Permissions { get; set; }
    public virtual ICollection<AppUser> Users { get; set; }
}
