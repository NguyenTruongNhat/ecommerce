using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class WebsocketConfiguration : IEntityTypeConfiguration<Websocket>
{
    public void Configure(EntityTypeBuilder<Websocket> builder)
    {
        builder.ToTable("Websocket");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();


        //builder.HasOne(x => x.User)
        //    .WithMany(x => x.WebSockets)
        //    .HasForeignKey(x => x.UserId)
        //    .OnDelete(DeleteBehavior.Cascade);
    }
}
