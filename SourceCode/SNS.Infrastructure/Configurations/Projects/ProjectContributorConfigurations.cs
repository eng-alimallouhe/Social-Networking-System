using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Projects.Bridges;

namespace SNS.Infrastructure.Configurations.Projects;

public class ProjectContributorConfigurations :
    IEntityTypeConfiguration<ProjectContributor>
{
    public void Configure(EntityTypeBuilder<ProjectContributor> builder)
    {
        builder.ToTable("ProjectContributors");

        builder.HasKey(pc => pc.Id);
        builder.HasIndex(pc => pc.ProjectId);
        builder.HasIndex(pc => pc.ContributorId);

        // Standard Unique Index: A user can only be a contributor once (Hard Delete entity)
        builder.HasIndex(
            pc => new 
            { pc.ProjectId, 
                pc.ContributorId 
            })
            .IsUnique();

        builder.Property(pc => pc.InvitationMessage)
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");

        builder.Property(pc => pc.InvitingStatus).HasConversion<int>();
        builder.Property(pc => pc.Role).HasConversion<int>();

        builder.HasOne(pc => pc.Project)
               .WithMany(p => p.Contributors)
               .HasForeignKey(pc => pc.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Contributor)
               .WithMany(p => p.Contributors)
               .HasForeignKey(pc => pc.ContributorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}