using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.Projects.Configurations;

public class ProjectRatingConfigurations :
    IEntityTypeConfiguration<ProjectRating>
{
    public void Configure(EntityTypeBuilder<ProjectRating> builder)
    {
        builder.ToTable("ProjectRatings", "Projects");

        builder.HasKey(pr => pr.Id);
        builder.HasIndex(pr => pr.ProjectId);
        builder.HasIndex(pr => pr.RaterId);

        // Standard Unique Index
        builder.HasIndex(
            pr => new 
            { 
                pr.ProjectId, 
                pr.RaterId 
            })
            .IsUnique();

        builder.Property(pr => pr.Comment)
               .HasMaxLength(1000)
               .HasColumnType("nvarchar(1000)");

        builder.HasOne<Project>()
               .WithMany(p => p.Ratings)
               .HasForeignKey(pr => pr.ProjectId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Profile>(pr => pr.Rater)
               .WithMany()
               .HasForeignKey(pr => pr.RaterId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
