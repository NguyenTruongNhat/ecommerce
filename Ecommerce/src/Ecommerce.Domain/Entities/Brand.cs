using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities;

public class Brand : DomainEntity<int>
{
    public string Logo { get; set; }
    public string Name { get; set; }

    // Navigation properties
    public virtual ICollection<Product> Products { get; set; }
    public virtual ICollection<BrandTranslation> BrandTranslations { get; set; }
}
