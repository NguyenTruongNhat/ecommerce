using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;

internal class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable(TableNames.Language);
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(500).IsRequired();        
    }
}
