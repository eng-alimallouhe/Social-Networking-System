using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ProfileContext.Bridges;

namespace SNS.Infrastructure.Configurations.ProfileContext;

public class ProfileSkillConfigurations :
    IEntityTypeConfiguration<ProfileSkill>
{
    public void Configure(EntityTypeBuilder<ProfileSkill> builder)
    {
        builder.ToTable("ProfileSkills");

        builder.HasKey(ps => ps.Id);

        builder.HasIndex(ps => ps.ProfileId);
        builder.HasIndex(ps => ps.SkillId);

        // Unique Constraint
        builder.HasIndex(
            ps => new 
            { 
                ps.ProfileId,
                ps.SkillId 
            })
            .IsUnique();

        builder.Property(ps => ps.Level)
               .HasConversion<int>();

        // Relationships
        builder.HasOne(ps => ps.Profile)
               .WithMany(p => p.ProfileSkills)
               .HasForeignKey(ps => ps.ProfileId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.Skill)
               .WithMany()
               .HasForeignKey(ps => ps.SkillId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}