using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable(TableNames.PaymentTransaction);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Gateway).HasMaxLength(100).IsRequired();
        builder.Property(x => x.TransactionDate).HasDefaultValueSql("GETDATE()");
        builder.Property(x => x.AccountNumber).HasMaxLength(100);
        builder.Property(x => x.SubAccount).HasMaxLength(250);
        builder.Property(x => x.Code).HasMaxLength(250);
        builder.Property(x => x.TransactionContent).HasColumnType("nvarchar(max)");
        builder.Property(x => x.ReferenceNumber).HasMaxLength(255);
        builder.Property(x => x.Body).HasColumnType("nvarchar(max)");

    }
}
