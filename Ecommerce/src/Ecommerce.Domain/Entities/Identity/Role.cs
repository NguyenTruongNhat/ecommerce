using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities.Identity;

public class Role : DomainEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<Permission> Permissions { get; set; }
    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<PermissionsRoles> PermissionsRoles { get; set; }
}
