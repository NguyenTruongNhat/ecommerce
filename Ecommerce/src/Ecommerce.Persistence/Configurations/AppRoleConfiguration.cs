using Ecommerce.Domain.Entities.Identity;
using Ecommerce.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;

internal sealed class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable(TableNames.AppRoles);

        builder.HasKey(x => x.Id);


        // Each User can have many RoleClaims
        builder.HasMany(e => e.Claims)
            .WithOne()
            .HasForeignKey(uc => uc.RoleId)
            .IsRequired();

    }
}
