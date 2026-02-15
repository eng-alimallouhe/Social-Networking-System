using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Content.Entities;

namespace SNS.Infrastructure.Configurations.Posts;

public class PostMediaConfigurations :
    IEntityTypeConfiguration<PostMedia>
{
    public void Configure(EntityTypeBuilder<PostMedia> builder)
    {
        builder.ToTable("PostMedias");

        builder.HasKey(pm => pm.Id);
        builder.HasIndex(pm => pm.PostId);

        builder.Property(pm => pm.Url)
               .IsRequired()
               .HasMaxLength(512)
               .HasColumnType("varchar(512)");

        builder.Property(pm => pm.ThumbnailUrl)
               .HasMaxLength(512)
               .HasColumnType("varchar(512)");

        builder.Property(pm => pm.MimeType)
               .HasMaxLength(100)
               .HasColumnType("varchar(100)");

        builder.Property(pm => pm.Type).HasConversion<int>();

        builder.HasOne(pm => pm.Post)
               .WithMany(p => p.Media)
               .HasForeignKey(pm => pm.PostId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}