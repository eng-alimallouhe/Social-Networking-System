using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.ContentManagement.Posts.Entities;

namespace SNS.Infrastructure.ContentManagement.Posts.Configurations;

public class PostTagConfigurations :
    IEntityTypeConfiguration<PostTag>
{
    public void Configure(EntityTypeBuilder<PostTag> builder)
    {
        builder.ToTable("PostTags", "ContentManagement");

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

        builder.HasOne<Post>()
               .WithMany(p => p.PostTags)
               .HasForeignKey(pt => pt.PostId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pt => pt.Tag)
               .WithMany()
               .HasForeignKey(pt => pt.TagId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
