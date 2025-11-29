using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities;

public class ProductTranslation : DomainEntity<int>
{ 
    public Guid ProductId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; }
    public virtual Language Language { get; set; }
}
