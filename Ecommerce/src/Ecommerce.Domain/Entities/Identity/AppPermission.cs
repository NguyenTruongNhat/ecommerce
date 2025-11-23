using Ecommerce.Contract.Enumerations;

namespace Ecommerce.Domain.Entities.Identity;

public class AppPermission 
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Path { get; set; }
    public HTTPMethod Method { get; set; }
    public string Module { get; set; }

    // Navigation properties

    public virtual ICollection<AppRole> Roles { get; set; }
}
