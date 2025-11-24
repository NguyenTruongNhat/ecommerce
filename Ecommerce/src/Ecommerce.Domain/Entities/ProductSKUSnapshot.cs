namespace Ecommerce.Domain.Entities;

public class ProductSKUSnapshot
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public double SkuPrice { get; set; }
    public string Image { get; set; }
    public string SkuValue { get; set; }
    public int? SkuId { get; set; }
    public int? OrderId { get; set; }
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
    public string ProductTranslations { get; set; }

    // Navigation properties
    public virtual SKU Sku { get; set; }
    public virtual Order Order { get; set; }
    public virtual Product Product { get; set; }
}
