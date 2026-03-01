using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.Projects;

public class ProjectSkillConfigurations : 
    IEntityTypeConfiguration<ProjectSkill>
{
    public void Configure(EntityTypeBuilder<ProjectSkill> builder)
    {
        builder.ToTable("ProjectSkills");

        builder.HasKey(ps => ps.Id);

        builder.HasIndex(ps => ps.ProjectId);

        builder.HasIndex(ps => ps.SkillId);

        builder.HasIndex(
            ps => new 
            { 
                ps.ProjectId, 
                ps.SkillId 
            }).IsUnique();

        builder.HasOne<SNS.Domain.Projects.Entities.Project>()
               .WithMany()
               .HasForeignKey(ps => ps.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<SNS.Domain.Preferences.Entities.Skill>()
               .WithMany()
               .HasForeignKey(ps => ps.SkillId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}