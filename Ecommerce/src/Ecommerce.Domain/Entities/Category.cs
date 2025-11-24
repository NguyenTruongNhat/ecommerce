namespace Ecommerce.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Logo { get; set; }
    public int? ParentCategoryId { get; set; }

    // Navigation properties
    public virtual Category ParentCategory { get; set; }
    public virtual ICollection<Product> Products { get; set; }
    public virtual ICollection<Category> ChildrenCategories { get; set; }
    public virtual ICollection<CategoryTranslation> CategoryTranslations { get; set; }
}
