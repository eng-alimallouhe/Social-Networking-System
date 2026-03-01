using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Posts.Bridges;

namespace SNS.Infrastructure.Configurations.Posts;

public class PostTagConfigurations :
    IEntityTypeConfiguration<PostTag>
{
    public void Configure(EntityTypeBuilder<PostTag> builder)
    {
        builder.ToTable("PostTags");

        builder.HasKey(x => x.Id);

        // Composite Primary Key (No Id property in entity)
        builder.HasIndex(
            pt => 
            new 
            { 
                pt.PostId, 
                pt.TagId 
            })
            .IsUnique();

        builder.Property(pt => pt.Confidence).HasColumnType("real");

        builder.HasOne<SNS.Domain.Content.Entities.Post>()
               .WithMany()
               .HasForeignKey(pt => pt.PostId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<SNS.Domain.Preferences.Entities.Tag>()
               .WithMany()
               .HasForeignKey(pt => pt.TagId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}