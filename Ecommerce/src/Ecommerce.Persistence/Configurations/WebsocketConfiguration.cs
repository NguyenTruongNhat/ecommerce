using Ecommerce.Domain.Entities;
using Ecommerce.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class WebsocketConfiguration : IEntityTypeConfiguration<Websocket>
{
    public void Configure(EntityTypeBuilder<Websocket> builder)
    {
        builder.ToTable(TableNames.Websocket);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();
    }
}
