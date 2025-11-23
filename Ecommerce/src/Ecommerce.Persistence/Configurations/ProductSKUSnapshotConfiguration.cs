using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class ProductSKUSnapshotConfiguration : IEntityTypeConfiguration<ProductSKUSnapshot>
{
    public void Configure(EntityTypeBuilder<ProductSKUSnapshot> builder)
    {
        builder.ToTable("ProductSKUSnapshot");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductName).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Image).IsRequired();
        builder.Property(x => x.SkuValue).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ProductTranslations).HasColumnType("nvarchar(max)");


        builder.HasOne(x => x.Sku)
            .WithMany(x => x.ProductSKUSnapshots)
            .HasForeignKey(x => x.SkuId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.ProductSKUSnapshots)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
