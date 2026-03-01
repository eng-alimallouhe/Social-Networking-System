using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Posts.Bridges;

namespace SNS.Infrastructure.Configurations.Posts;

public class PostTopicConfigurations :
    IEntityTypeConfiguration<PostTopic>
{
    public void Configure(EntityTypeBuilder<PostTopic> builder)
    {
        builder.ToTable("PostTopics");

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

        builder.HasOne<SNS.Domain.Content.Entities.Post>()
               .WithMany()
               .HasForeignKey(pt => pt.PostId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<SNS.Domain.Preferences.Entities.Topic>()
               .WithMany()
               .HasForeignKey(pt => pt.TopicId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}