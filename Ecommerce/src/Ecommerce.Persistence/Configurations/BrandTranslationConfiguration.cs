using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class BrandTranslationConfiguration : IEntityTypeConfiguration<BrandTranslation>
{
    public void Configure(EntityTypeBuilder<BrandTranslation> builder)
    {
        builder.ToTable("BrandTranslation");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Description).IsRequired();

        builder.HasOne(x => x.Brand)
            .WithMany(x => x.BrandTranslations)
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Language)
            .WithMany(x => x.BrandTranslations)
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
