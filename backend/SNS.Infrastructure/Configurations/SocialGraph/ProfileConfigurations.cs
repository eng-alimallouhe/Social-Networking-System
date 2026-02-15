using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Security.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Infrastructure.Configurations.SocialGraph;

public class ProfileConfigurations : 
    IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles");

        builder.HasKey(p => p.Id);

        // Foreign Key Indexes
        builder.HasIndex(p => p.UserId).IsUnique(); // One-to-One with User
        builder.HasIndex(p => p.FacultyId);
        builder.HasIndex(p => p.UniversityId);

        // Search Index
        builder.HasIndex(p => p.FullName);

        // Properties
        builder.Property(p => p.FullName)
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(p => p.Bio)
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");

        builder.Property(p => p.Specialization)
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(p => p.Location)
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(p => p.SkillsSummary)
               .HasMaxLength(1000)
               .HasColumnType("nvarchar(1000)");

        // URL Properties - Standardizing on 512 for links
        builder.Property(p => p.ProfilePictureUrl).HasMaxLength(512).HasColumnType("varchar(512)");
        builder.Property(p => p.GitHubUrl).HasMaxLength(512).HasColumnType("varchar(512)");
        builder.Property(p => p.LinkedInUrl).HasMaxLength(512).HasColumnType("varchar(512)");
        builder.Property(p => p.FacebookUrl).HasMaxLength(512).HasColumnType("varchar(512)");
        builder.Property(p => p.XUrl).HasMaxLength(512).HasColumnType("varchar(512)");
        builder.Property(p => p.Website).HasMaxLength(512).HasColumnType("varchar(512)");

        // Relationships

        // 1. One-to-One with User (Profile depends on User)
        builder.HasOne<User>()
               .WithOne(u => u.Profile)
               .HasForeignKey<Profile>(p => p.UserId)
               .IsRequired();

        // 2. Optional Education Relationships
        builder.HasOne(p => p.Faculty)
               .WithMany(f => f.Profiles)
               .HasForeignKey(p => p.FacultyId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.University)
               .WithMany(u => u.Profiles)
               .HasForeignKey(p => p.UniversityId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}