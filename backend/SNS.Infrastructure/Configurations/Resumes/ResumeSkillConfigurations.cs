using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Resumes.Bridges;

namespace SNS.Infrastructure.Configurations.Resumes;

public class ResumeSkillConfigurations : 
    IEntityTypeConfiguration<ResumeSkill>
{
    public void Configure(EntityTypeBuilder<ResumeSkill> builder)
    {
        builder.ToTable("ResumeSkills");

        builder.HasKey(rs => rs.Id);
        builder.HasIndex(rs => rs.ResumeId);

        // Composite index: Don't list the same skill twice for one resume
        builder.HasIndex(rs => new { rs.ResumeId, rs.SkillName }).IsUnique();

        builder.Property(rs => rs.SkillName)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(rs => rs.Level)
               .HasConversion<int>();

        builder.HasOne(rs => rs.Resume)
               .WithMany(r => r.Skills)
               .HasForeignKey(rs => rs.ResumeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}