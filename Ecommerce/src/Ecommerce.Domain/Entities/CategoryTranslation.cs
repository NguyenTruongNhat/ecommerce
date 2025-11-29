using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities;

public class CategoryTranslation : DomainEntity<int>
{
    public int CategoryId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public virtual Category Category { get; set; }
    public virtual Language Language { get; set; }
}
