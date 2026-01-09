using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Jobs.Entities;

namespace SNS.Infrastructure.Configurations.Jobs;

public class JobApplicationConfiguration :
    IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("JobApplications");

        builder.HasKey(ja => ja.Id);

        builder.HasQueryFilter(ja => ja.IsActive);

        builder.Property(ja => ja.Status).HasConversion<int>();
        builder.Property(ja => ja.ResumeFileUrl).HasMaxLength(500);

        // Prevent duplicate active applications from the same profile
        builder.HasIndex(
            ja => new 
            { 
                ja.JobId, 
                ja.ApplicantId 
            })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.HasOne(ja => ja.Job)
               .WithMany(j => j.Applications)
               .HasForeignKey(ja => ja.JobId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ja => ja.Applicant)
               .WithMany() // Or p.JobApplications if defined in Profile
               .HasForeignKey(ja => ja.ApplicantId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}