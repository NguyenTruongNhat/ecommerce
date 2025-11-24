using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable(TableNames.Message);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content).IsRequired();

        builder.HasOne(x => x.FromUser)
            .WithMany(x => x.SentMessages)
            .HasForeignKey(x => x.FromUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ToUser)
            .WithMany(x => x.ReceivedMessages)
            .HasForeignKey(x => x.ToUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
