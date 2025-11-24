using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities.Identity;

public class Permission : DomainEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Path { get; set; }
    public HTTPMethod Method { get; set; }
    public virtual ICollection<PermissionsRoles> PermissionsRoles { get; set; }
}
