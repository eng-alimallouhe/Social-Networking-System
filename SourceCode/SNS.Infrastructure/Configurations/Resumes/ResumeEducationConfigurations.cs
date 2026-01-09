using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Resumes.Entities;

namespace SNS.Infrastructure.Configurations.Resumes;

public class ResumeEducationConfigurations :
    IEntityTypeConfiguration<ResumeEducation>
{
    public void Configure(EntityTypeBuilder<ResumeEducation> builder)
    {
        builder.ToTable("ResumeEducations");

        builder.HasKey(re => re.Id);
        builder.HasIndex(re => re.ResumeId);

        builder.Property(re => re.UniversityName)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(re => re.FacultyName)
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(re => re.Degree)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(re => re.FieldOfStudy)
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.HasOne(re => re.Resume)
               .WithMany(r => r.Educations)
               .HasForeignKey(re => re.ResumeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); // Deleting a resume should delete its education entries
    }
}