using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ProfileContext.Bridges;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.ProfileContext;

public class ProfileTopicConfigurations : 
    IEntityTypeConfiguration<ProfileTopic>
{
    public void Configure(EntityTypeBuilder<ProfileTopic> builder)
    {
        builder.ToTable("ProfileTopics");

        builder.HasKey(pt => pt.Id);

        builder.HasIndex(pt => pt.ProfileId);
        builder.HasIndex(pt => pt.TopicId);

        // Unique Constraint
        builder.HasIndex(
            pt => new 
            { 
                pt.ProfileId, 
                pt.TopicId 
            })
            .IsUnique();

        builder.Property(pt => pt.Score)
               .HasColumnType("float");

        // Relationships
        builder.HasOne(pt => pt.Profile)
               .WithMany(p => p.ProfileTopics)
               .HasForeignKey(pt => pt.ProfileId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pt => pt.Topic)
               .WithMany()
               .HasForeignKey(pt => pt.TopicId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}