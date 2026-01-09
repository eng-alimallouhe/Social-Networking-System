using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Projects.Entities;

namespace SNS.Infrastructure.Configurations.Projects;

public class ProjectMilestoneConfigurations :
    IEntityTypeConfiguration<ProjectMilestone>
{
    public void Configure(EntityTypeBuilder<ProjectMilestone> builder)
    {
        builder.ToTable("ProjectMilestones");

        builder.HasKey(pm => pm.Id);
        builder.HasIndex(pm => pm.ProjectId);

        builder.Property(pm => pm.Title)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(pm => pm.Description)
               .HasMaxLength(1000)
               .HasColumnType("nvarchar(1000)");

        builder.HasOne(pm => pm.Project)
               .WithMany(p => p.Milestones)
               .HasForeignKey(pm => pm.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}