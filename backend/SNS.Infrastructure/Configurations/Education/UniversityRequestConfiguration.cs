using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Education.Entities;

namespace SNS.Infrastructure.Configurations.Education;

public class UniversityRequestConfiguration :
    IEntityTypeConfiguration<UniversityRequest>
{
    public void Configure(EntityTypeBuilder<UniversityRequest> builder)
    {
        builder.ToTable("UniversityRequests");

        builder.HasKey(ur => ur.Id);
        builder.HasIndex(ur => ur.SubmitterId);

        // Note: No IsActive property in UniversityRequest, so no QueryFilter here.

        builder.Property(ur => ur.Name)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(ur => ur.Country)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(ur => ur.City)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(ur => ur.ReviewComment)
               .HasMaxLength(500)
               .HasColumnType("nvarchar(500)");

        builder.Property(ur => ur.Status).HasConversion<int>();

        builder.HasOne(ur => ur.Submitter)
               .WithMany()
               .HasForeignKey(ur => ur.SubmitterId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}