using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Education.Entities;

namespace SNS.Infrastructure.Configurations.Education;

public class FacultyConfiguration :
    IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.ToTable("Faculties");

        builder.HasKey(f => f.Id);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(f => f.IsActive);

        builder.HasIndex(f => f.UniversityId);

        // Unique Constraint: A University shouldn't have duplicate Faculty names (active only)
        builder.HasIndex(f => new { f.UniversityId, f.Name })
               .IsUnique()
               .HasFilter("[IsActive] = 1");

        builder.Property(f => f.Name)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.HasOne(f => f.University)
               .WithMany(u => u.Faculties)
               .HasForeignKey(f => f.UniversityId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}