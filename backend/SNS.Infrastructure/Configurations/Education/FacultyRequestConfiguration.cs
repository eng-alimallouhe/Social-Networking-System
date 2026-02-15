using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Education.Entities;

namespace SNS.Infrastructure.Configurations.Education;

public class FacultyRequestConfiguration :
    IEntityTypeConfiguration<FacultyRequest>
{
    public void Configure(EntityTypeBuilder<FacultyRequest> builder)
    {
        builder.ToTable("FacultyRequests");

        builder.HasKey(fr => fr.Id);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(fr => fr.IsActive);

        builder.HasIndex(fr => fr.SubmitterId);
        builder.HasIndex(fr => fr.UniversityId);
        builder.HasIndex(fr => fr.UniversityRequestId);

        builder.Property(fr => fr.Name)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(fr => fr.ReviewComment)
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");

        builder.Property(fr => fr.Status).HasConversion<int>();

        // Relationships
        builder.HasOne(fr => fr.Submitter)
               .WithMany()
               .HasForeignKey(fr => fr.SubmitterId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(fr => fr.University)
               .WithMany(u => u.FacultyRequests)
               .HasForeignKey(fr => fr.UniversityId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(fr => fr.UniversityRequest)
               .WithMany() // Assuming UniversityRequest doesn't have FacultyRequests collection
               .HasForeignKey(fr => fr.UniversityRequestId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}