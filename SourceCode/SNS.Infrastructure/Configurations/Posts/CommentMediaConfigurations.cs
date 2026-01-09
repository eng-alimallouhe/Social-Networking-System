using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Posts.Entities;

namespace SNS.Infrastructure.Configurations.Posts;

public class CommentMediaConfigurations :
    IEntityTypeConfiguration<CommentMedia>
{
    public void Configure(EntityTypeBuilder<CommentMedia> builder)
    {
        builder.ToTable("CommentMedias");

        builder.HasKey(cm => cm.Id);
        builder.HasIndex(cm => cm.CommentId);

        builder.Property(cm => cm.Url)
               .IsRequired()
               .HasMaxLength(512)
               .HasColumnType("varchar(512)");

        builder.Property(cm => cm.MimeType)
               .HasMaxLength(100)
               .HasColumnType("varchar(100)");

        builder.Property(cm => cm.Type).HasConversion<int>();

        builder.HasOne(cm => cm.Comment)
               .WithMany(c => c.Medias)
               .HasForeignKey(cm => cm.CommentId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}