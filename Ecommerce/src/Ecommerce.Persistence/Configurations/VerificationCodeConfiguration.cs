using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.ToTable(TableNames.VerificationCode);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();

        builder.HasIndex(x => new { x.Email, x.Type }).IsUnique();
        builder.HasIndex(x => x.ExpiresAt);
    }
}
