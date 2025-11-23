using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;

internal class UserTranslationConfiguration : IEntityTypeConfiguration<UserTranslation>
{
    public void Configure(EntityTypeBuilder<UserTranslation> builder)
    {
        builder.ToTable("UserTranslation");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Address).HasMaxLength(500);
        
        builder.HasOne(x => x.Language)
            .WithMany(x => x.UserTranslations)
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}
