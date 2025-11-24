using Ecommerce.Domain.Entities.Identity;
using Ecommerce.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;



internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableNames.User);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Password).HasMaxLength(255).IsRequired();
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);


        builder.HasOne(x => x.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(TableNames.Role);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);

        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}


internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(TableNames.Permission);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Path).HasMaxLength(500).IsRequired();

        builder.Property(x => x.Method)
            .HasConversion<string>() // Ánh xạ Enum sang string trong DB
            .HasMaxLength(10);
    }
}

internal class PermissionsRolesConfiguration : IEntityTypeConfiguration<PermissionsRoles>
{
    public void Configure(EntityTypeBuilder<PermissionsRoles> builder)
    {
        builder.ToTable(TableNames.PermissionsRoles);

        builder.HasKey(x => new { x.PermissionId, x.RoleId });

        builder.HasOne(x => x.Permission)
            .WithMany(p => p.PermissionsRoles)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(x => x.Role)
            .WithMany(r => r.PermissionsRoles)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
