using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class SKUConfiguration : IEntityTypeConfiguration<SKU>
{
    public void Configure(EntityTypeBuilder<SKU> builder)
    {
        builder.ToTable("SKU");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Image).IsRequired();



        builder.HasIndex(x => x.ProductId);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.Skus)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
