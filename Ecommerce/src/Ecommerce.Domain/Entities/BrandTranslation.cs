using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities;

public class BrandTranslation : DomainEntity<Guid>
{
    public int BrandId { get; set; }
    public string LanguageId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public virtual Brand Brand { get; set; }
    public virtual Language Language { get; set; }
}
