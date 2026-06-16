using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Educations.Entities;

namespace SNS.Infrastructure.Education.Configurations;

public class AcademicRecordConfigurations :
    IEntityTypeConfiguration<AcademicRecord>
{
    public void Configure(EntityTypeBuilder<AcademicRecord> builder)
    {
        builder.ToTable("AcademicRecords", "Education");

        builder.HasKey(f => f.Id);

        builder.HasIndex(f => f.UniversityId);

        // Unique Constraint: A University shouldn't have duplicate Faculty names (active only)
        builder.HasIndex(f => new { f.UniversityId, f.ProfileId })
               .IsUnique();

        builder.Property(f => f.Degree)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");


        builder.Property(f => f.FieldOfStudy)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");


        builder.Property(f => f.Grade)
               .IsRequired()
               .HasMaxLength(50)
               .HasColumnType("nvarchar(50)");



        builder.Property(f => f.Description)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");


        builder.HasOne(e => e.University)
               .WithMany()
               .HasForeignKey(f => f.UniversityId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(e => e.Profile)
               .WithMany(p => p.AcademicRecords)
               .HasForeignKey(f => f.ProfileId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
