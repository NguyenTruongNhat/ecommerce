using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities;

public class CategoryTranslation : DomainEntity<Guid>
{
    public int CategoryId { get; set; }
    public string LanguageId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public virtual Category Category { get; set; }
    public virtual Language Language { get; set; }
}
