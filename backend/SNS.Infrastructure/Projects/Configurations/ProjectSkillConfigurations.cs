using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Projects.Entities;

namespace SNS.Infrastructure.Projects.Configurations;

public class ProjectSkillConfigurations : 
    IEntityTypeConfiguration<ProjectSkill>
{
    public void Configure(EntityTypeBuilder<ProjectSkill> builder)
    {
        builder.ToTable("ProjectSkills", "Projects");

        builder.HasKey(ps => ps.Id);

        builder.HasIndex(ps => ps.ProjectId);

        builder.HasIndex(ps => ps.SkillId);

        builder.HasIndex(
            ps => new 
            { 
                ps.ProjectId, 
                ps.SkillId 
            }).IsUnique();

        builder.HasOne<Project>()
               .WithMany(p => p.Skills)
               .HasForeignKey(ps => ps.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.Skill)
               .WithMany()
               .HasForeignKey(ps => ps.SkillId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
