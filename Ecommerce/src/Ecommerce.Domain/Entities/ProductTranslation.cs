namespace Ecommerce.Domain.Entities;

public class ProductTranslation
{
    public int Id { get; set; }
    public Guid ProductId { get; set; }
    public string LanguageId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; }
    public virtual Language Language { get; set; }
}
