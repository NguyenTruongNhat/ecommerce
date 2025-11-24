namespace Ecommerce.Domain.Entities.Identity;
public class PermissionsRoles
{
    public int PermissionId { get; set; }
    public virtual Permission Permission { get; set; }

    public int RoleId { get; set; }
    public virtual Role Role { get; set; }
}
