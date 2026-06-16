using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Resumes.Entities;

namespace SNS.Infrastructure.Resumes.Configurations;

public class ResumeProjectConfigurations :
    IEntityTypeConfiguration<ResumeProject>
{
    public void Configure(EntityTypeBuilder<ResumeProject> builder)
    {
        builder.ToTable("ResumeProjects", "Resumes");

        // Composite Primary Key
        builder.HasKey(
            rp => new 
            { 
                rp.ResumeId, 
                rp.ProjectId 
            });

        builder.HasOne<Resume>()
               .WithMany(r => r.Projects)
               .HasForeignKey(rp => rp.ResumeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
