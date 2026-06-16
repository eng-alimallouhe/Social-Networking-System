using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ContentManagement.Posts.Entities;

namespace SNS.Infrastructure.ContentManagement.Posts.Configurations;

public class PostTopicConfigurations :
    IEntityTypeConfiguration<PostTopic>
{
    public void Configure(EntityTypeBuilder<PostTopic> builder)
    {
        builder.ToTable("PostTopics", "ContentManagement");

        builder.HasKey(pt => pt.Id);

        // Composite Primary Key
        builder.HasIndex(
            pt => 
            new 
            { 
                pt.PostId, 
                pt.TopicId 
            })
            .IsUnique();

        builder.Property(pt => pt.Confidence).HasColumnType("real");

        builder.HasOne(t => t.Post)
               .WithMany(p => p.PostTopics)
               .HasForeignKey(pt => pt.PostId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Topic)
               .WithMany()
               .HasForeignKey(pt => pt.TopicId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
