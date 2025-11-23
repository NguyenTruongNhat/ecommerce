using Ecommerce.Domain.Entities.Identity;
using Ecommerce.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;

internal sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable(TableNames.AppUsers);

        builder.HasKey(x => x.Id);


        // Each User can have many UserClaims
        builder.HasMany(e => e.Claims)
            .WithOne()
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();

        // Each User can have many UserLogins
        builder.HasMany(e => e.Logins)
            .WithOne()
            .HasForeignKey(ul => ul.UserId)
            .IsRequired();

        // Each User can have many UserTokens
        builder.HasMany(e => e.Tokens)
            .WithOne()
            .HasForeignKey(ut => ut.UserId)
            .IsRequired();

    }
}
