using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Resumes.Entities;

namespace SNS.Infrastructure.Configurations.Resumes;

public class ResumeCertificateConfigurations :
    IEntityTypeConfiguration<ResumeCertificate>
{
    public void Configure(EntityTypeBuilder<ResumeCertificate> builder)
    {
        builder.ToTable("ResumeCertificates");

        builder.HasKey(rc => rc.Id);
        builder.HasIndex(rc => rc.ResumeId);

        builder.Property(rc => rc.Title)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(rc => rc.Issuer)
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.HasOne(rc => rc.Resume)
               .WithMany(r => r.Certificates)
               .HasForeignKey(rc => rc.ResumeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}