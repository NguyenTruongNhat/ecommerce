using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brand");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Logo).HasMaxLength(1000).IsRequired();

    }
}
