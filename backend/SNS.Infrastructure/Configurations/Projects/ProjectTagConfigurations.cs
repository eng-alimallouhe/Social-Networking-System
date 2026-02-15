using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.Projects;

public class ProjectTagConfigurations : 
    IEntityTypeConfiguration<ProjectTag>
{
    public void Configure(EntityTypeBuilder<ProjectTag> builder)
    {
        builder.ToTable("ProjectTags");

        builder.HasKey(pt => pt.Id);
        builder.HasIndex(pt => pt.ProjectId);
        builder.HasIndex(pt => pt.TagId);

        builder.HasIndex(pt => new { pt.ProjectId, pt.TagId }).IsUnique();

        builder.HasOne(pt => pt.Project)
               .WithMany(p => p.Tags)
               .HasForeignKey(pt => pt.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pt => pt.Tag)
               .WithMany()
               .HasForeignKey(pt => pt.TagId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}