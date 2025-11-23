using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configurations;
internal class ReviewMediaConfiguration : IEntityTypeConfiguration<ReviewMedia>
{
    public void Configure(EntityTypeBuilder<ReviewMedia> builder)
    {
        builder.ToTable("ReviewMedia");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url).HasMaxLength(1000).IsRequired();


        builder.HasOne(x => x.Review)
            .WithMany(x => x.Medias)
            .HasForeignKey(x => x.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
