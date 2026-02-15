using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Projects.Entities;

namespace SNS.Infrastructure.Configurations.Projects;

public class ProjectMediaConfigurations : 
    IEntityTypeConfiguration<ProjectMedia>
{
    public void Configure(EntityTypeBuilder<ProjectMedia> builder)
    {
        builder.ToTable("ProjectMedias");

        builder.HasKey(pm => pm.Id);
        builder.HasIndex(pm => pm.ProjectId);

        builder.Property(pm => pm.MediaUrl)
               .IsRequired()
               .HasMaxLength(512)
               .HasColumnType("varchar(512)");

        builder.Property(pm => pm.Caption)
               .HasMaxLength(255)
               .HasColumnType("nvarchar(255)");

        builder.Property(pm => pm.Type)
               .HasConversion<int>();

        builder.HasOne(pm => pm.Project)
               .WithMany(p => p.Media)
               .HasForeignKey(pm => pm.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}