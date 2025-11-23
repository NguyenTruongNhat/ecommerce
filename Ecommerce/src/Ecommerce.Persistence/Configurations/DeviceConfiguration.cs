using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Device");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserAgent).IsRequired();
        builder.Property(x => x.Ip).IsRequired();
        builder.Property(x => x.IsActive).HasDefaultValue(true);


        builder.HasOne(x => x.User)
            .WithMany(x => x.Devices)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
