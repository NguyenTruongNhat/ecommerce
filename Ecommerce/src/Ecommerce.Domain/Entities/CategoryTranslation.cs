namespace Ecommerce.Domain.Entities;

public class CategoryTranslation
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string LanguageId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public virtual Category Category { get; set; }
    public virtual Language Language { get; set; }
}
