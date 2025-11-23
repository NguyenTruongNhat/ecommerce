using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Variants).HasColumnType("nvarchar(max)");



        builder.HasOne(x => x.Brand)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Categories)
            .WithMany(x => x.Products)
            .UsingEntity(j => j.ToTable("ProductCategory"));
    }
}
