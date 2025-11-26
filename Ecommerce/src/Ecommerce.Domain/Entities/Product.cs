using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities;

public class Product : DomainEntity<Guid>
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string Description { get; private set; }
    public DateTime? PublishedAt { get; set; }
    public double BasePrice { get; set; }
    public double VirtualPrice { get; set; }
    public int? BrandId { get; set; }
    public string[] Images { get; set; } = new string[0];
    public string? Variants { get; set; }
    public int? CreatedById { get; set; }
    public int? UpdatedById { get; set; }
    public int? DeletedById { get; set; }

    // Navigation properties
    public virtual Brand Brand { get; set; }
    public virtual ICollection<Category> Categories { get; set; }
    public virtual ICollection<SKU> Skus { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
    public virtual ICollection<ProductTranslation> ProductTranslations { get; set; }
    public virtual ICollection<Order> Orders { get; set; }
    public virtual ICollection<ProductSKUSnapshot> ProductSKUSnapshots { get; set; }

    public static Product CreateProduct(Guid id, string name, decimal price, string description, string variants)
    {
        return new Product(id, name, price, description, variants);
    }

    public Product(Guid id, string name, decimal price, string description, string variants)
    {
        //if (!NameValidation(name))
        //    throw new ArgumentNullException();
        Id = id;
        Name = name;
        Price = price;
        Description = description;
        Variants = variants;
    }

    public void Update(string name, decimal price, string description)
    {
        //if (!NameValidation(name))
        //    throw new ArgumentNullException();

        Name = name;
        Price = price;
        Description = description;
    }

    private bool NameValidation(string name)
        => name.Contains("ABCD-");
}
