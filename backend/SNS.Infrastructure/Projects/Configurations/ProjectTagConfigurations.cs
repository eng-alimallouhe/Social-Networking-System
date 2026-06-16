using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Projects.Entities;

namespace SNS.Infrastructure.Projects.Configurations;

public class ProjectTagConfigurations : 
    IEntityTypeConfiguration<ProjectTag>
{
    public void Configure(EntityTypeBuilder<ProjectTag> builder)
    {
        builder.ToTable("ProjectTags", "Projects");

        builder.HasKey(pt => pt.Id);
        builder.HasIndex(pt => pt.ProjectId);
        builder.HasIndex(pt => pt.TagId);

        builder.HasIndex(pt => new { pt.ProjectId, pt.TagId }).IsUnique();

        builder.HasOne<Project>()
               .WithMany(p => p.Tags)
               .HasForeignKey(pt => pt.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tag>()
               .WithMany()
               .HasForeignKey(pt => pt.TagId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
