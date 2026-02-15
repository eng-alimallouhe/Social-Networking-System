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

        builder.HasOne(pt => pt.Post)
               .WithMany(p => p.Topics)
               .HasForeignKey(pt => pt.PostId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pt => pt.Topic)
               .WithMany()
               .HasForeignKey(pt => pt.TopicId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}