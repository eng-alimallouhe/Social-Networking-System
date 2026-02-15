using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Communities.Entities;

namespace SNS.Infrastructure.Configurations.Communities;

public class CommunityConfigurations :
    IEntityTypeConfiguration<Community>
{
    public void Configure(EntityTypeBuilder<Community> builder)
    {
        builder.ToTable("Communities");

        builder.HasKey(c => c.Id);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(c => c.IsActive);

        builder.HasIndex(c => c.OwnerId);
        builder.HasIndex(c => c.Name); 

        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(c => c.Description)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");

        builder.Property(c => c.RulesText)
               .HasColumnType("nvarchar(max)");

        builder.Property(c => c.LogoUrl)
               .HasMaxLength(512)
               .HasColumnType("varchar(512)");

        // Enums
        builder.Property(c => c.Policy).HasConversion<int>();
        builder.Property(c => c.Type).HasConversion<int>();
        builder.Property(c => c.Status).HasConversion<int>();

        // Relationships
        builder.HasOne(c => c.Owner)
               .WithMany(p => p.Communities)
               .HasForeignKey(c => c.OwnerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        // 1:1 Relationship with Settings
        builder.HasOne(c => c.Settings)
               .WithOne(s => s.Community)
               .HasForeignKey<CommunitySettings>(s => s.CommunityId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}