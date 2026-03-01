using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Jobs.Entities;

namespace SNS.Infrastructure.Configurations.Jobs;

public class JobSkillConfiguration : 
    IEntityTypeConfiguration<JobSkill>
{
    public void Configure(EntityTypeBuilder<JobSkill> builder)
    {
        builder.ToTable("JobSkills");

        builder.HasKey(js => js.Id);

        // Composite Index to ensure unique skill per job
        builder.HasIndex(
            js => new 
            { 
                js.JobId, 
                js.SkillId 
            })
            .IsUnique();

        builder.HasOne<SNS.Domain.Jobs.Entities.Job>()
               .WithMany()
               .HasForeignKey(js => js.JobId);

        builder.HasOne<SNS.Domain.Preferences.Entities.Skill>()
               .WithMany()
               .HasForeignKey(js => js.SkillId);
    }
}