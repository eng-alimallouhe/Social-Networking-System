using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.Profiles.Profiles.Configurations;

public class ProfileConfigurations : 
    IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles", "Profiles");

        builder.HasKey(p => p.Id);

        // Foreign Key Indexes
        builder.HasIndex(p => p.UserId).IsUnique();

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
        builder.HasOne(p => p.Owner)
               .WithOne(u => u.UserProfile)
               .HasForeignKey<Profile>(p => p.UserId)
               .IsRequired();
    }
}
