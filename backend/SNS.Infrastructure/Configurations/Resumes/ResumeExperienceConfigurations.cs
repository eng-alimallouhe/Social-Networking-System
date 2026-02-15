using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Resumes.Entities;

namespace SNS.Infrastructure.Configurations.Resumes;

public class ResumeExperienceConfigurations :
    IEntityTypeConfiguration<ResumeExperience>
{
    public void Configure(EntityTypeBuilder<ResumeExperience> builder)
    {
        builder.ToTable("ResumeExperiences");

        builder.HasKey(re => re.Id);
        builder.HasIndex(re => re.ResumeId);

        builder.Property(re => re.CompanyName)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(re => re.Position)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(re => re.Description)
               .HasMaxLength(4000) // Job descriptions can be very detailed
               .HasColumnType("nvarchar(4000)");

        builder.HasOne(re => re.Resume)
               .WithMany(r => r.Experiences)
               .HasForeignKey(re => re.ResumeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}