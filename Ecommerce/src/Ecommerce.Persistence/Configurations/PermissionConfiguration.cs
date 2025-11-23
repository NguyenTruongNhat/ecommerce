using Ecommerce.Domain.Entities.Identity;
using Ecommerce.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<AppPermission>
{
    public void Configure(EntityTypeBuilder<AppPermission> builder)
    {
        builder.ToTable(TableNames.Permissions);

    }
}
