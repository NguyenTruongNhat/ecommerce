using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(TableNames.Order);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Receiver).HasColumnType("nvarchar(max)");


        builder.HasMany(x => x.Products)
            .WithMany(x => x.Orders)
            .UsingEntity(j => j.ToTable("OrderProduct"));

        builder.HasOne(x => x.Payment)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
