using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Persistence;
public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

    public DbSet<User> Uses { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<PermissionsRoles> PermissionsRoles { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<ProductSKUSnapshot> ProductSKUSnapshots { get; set; }
    public DbSet<ProductTranslation> ProductTranslations { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<SKU> SKUs { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
