namespace Ecommerce.Domain.Entities;

public class Language
{
    public string Id { get; set; }
    public string Name { get; set; }

    // Navigation properties
    public virtual ICollection<UserTranslation> UserTranslations { get; set; }
    public virtual ICollection<ProductTranslation> ProductTranslations { get; set; }
    public virtual ICollection<CategoryTranslation> CategoryTranslations { get; set; }
    public virtual ICollection<BrandTranslation> BrandTranslations { get; set; }
}
