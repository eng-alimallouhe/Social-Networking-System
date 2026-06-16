using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.Jobs;

public class JobSkillConfiguration : 
    IEntityTypeConfiguration<JobSkill>
{
    public void Configure(EntityTypeBuilder<JobSkill> builder)
    {
        builder.ToTable("JobSkills", "Jobs");

        builder.HasKey(js => js.Id);

        // Composite Index to ensure unique skill per job
        builder.HasIndex(
            js => new 
            { 
                js.JobId, 
                js.SkillId 
            })
            .IsUnique();

        builder.HasOne<Job>()
               .WithMany(j => j.JobSkills)
               .HasForeignKey(js => js.JobId);

        builder.HasOne<Skill>()
               .WithMany()
               .HasForeignKey(js => js.SkillId);
    }
}
