using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable(TableNames.CartItem);
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.UserId, x.SkuId }).IsUnique();
        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.Sku)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.SkuId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Carts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

