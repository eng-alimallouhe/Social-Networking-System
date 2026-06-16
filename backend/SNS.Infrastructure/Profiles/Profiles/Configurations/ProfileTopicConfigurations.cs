using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Profiles.Profiles.Relations;

namespace SNS.Infrastructure.Profiles.Profiles.Configurations;

public class ProfileTopicConfigurations : 
    IEntityTypeConfiguration<ProfileTopic>
{
    public void Configure(EntityTypeBuilder<ProfileTopic> builder)
    {
        builder.ToTable("ProfileTopics", "ProfileContext");

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
        builder.HasOne<Profile>()
               .WithMany(p => p.ProfileTopics)
               .HasForeignKey(pt => pt.ProfileId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.Preferences.Entities.Topic>()
               .WithMany()
               .HasForeignKey(pt => pt.TopicId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
