using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.Preferences;

public class TopicInterestConfigurations :
    IEntityTypeConfiguration<TopicInterest>
{
    public void Configure(EntityTypeBuilder<TopicInterest> builder)
    {
        builder.ToTable("TopicInterests");

        builder.HasKey(ti => ti.Id);

        builder.HasIndex(ti => ti.TopicId);
        builder.HasIndex(ti => ti.InterestId);

        // Unique Bridge
        builder.HasIndex(ti => new { ti.TopicId, ti.InterestId }).IsUnique();

        builder.HasOne(ti => ti.Topic)
               .WithMany(t => t.TopicInterests)
               .HasForeignKey(ti => ti.TopicId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ti => ti.Interest)
               .WithMany(i => i.TopicInterests)
               .HasForeignKey(ti => ti.InterestId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}