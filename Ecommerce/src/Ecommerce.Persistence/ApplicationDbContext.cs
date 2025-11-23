using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Persistence;
public sealed class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

    public DbSet<AppUser> AppUses { get; set; }

    public DbSet<AppPermission> Permissions { get; set; }

    public DbSet<Product> Products { get; set; }
}
