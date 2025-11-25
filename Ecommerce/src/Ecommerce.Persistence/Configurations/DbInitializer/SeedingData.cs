using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations.DbInitializer;


internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
                Description = "Administrator role with full permissions",
                IsActive = true
            },
            new Role
            {
                Id = 2,
                Name = "Client",
                Description = "Client role for end users",
                IsActive = true
            },
            new Role
            {
                Id = 3,
                Name = "Seller",
                Description = "Seller role for merchants",
                IsActive = true
            }
        );
    }
}


internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasData(
            new User
            {
                Id = 1,
                Name = "System Administrator",
                Email = "admin@ecommerce.local",
                Password = "Admin@123",     // change to hashed password for production
                PhoneNumber = "0000000000",
                Avatar = string.Empty,
                TotpSecret = string.Empty,
                Status = UserStatus.ACTIVE,
                RoleId = 1
            }
        );
    }
}
