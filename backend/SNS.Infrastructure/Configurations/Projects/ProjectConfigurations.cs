using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Projects.Entities;

namespace SNS.Infrastructure.Configurations.Projects;

public class ProjectConfigurations : 
    IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.OwnerId);
        
        builder.HasIndex(p => p.Title); // Search index

        builder.Property(p => p.Title)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(p => p.ShortDescription)
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");

        // Large content
        builder.Property(p => p.ReadmeContent)
               .HasColumnType("nvarchar(max)");

        // URLs
        builder.Property(p => p.MainImageUrl)
            .HasMaxLength(512)
            .HasColumnType("varchar(512)");
        
        builder.Property(p => p.GitHubUrl)
            .HasMaxLength(512)
            .HasColumnType("varchar(512)");

        builder.Property(p => p.LiveDemoUrl)
            .HasMaxLength(512)
            .HasColumnType("varchar(512)");

        // Enums
        builder.Property(p => p.Type)
            .HasConversion<int>();

        builder.Property(p => p.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(p => p.Owner)
               .WithMany(p => p.Projects)
               .HasForeignKey(p => p.OwnerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}