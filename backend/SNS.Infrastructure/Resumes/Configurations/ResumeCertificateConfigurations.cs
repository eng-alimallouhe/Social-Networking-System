using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Resumes.Entities;

namespace SNS.Infrastructure.Resumes.Configurations;

public class ResumeCertificateConfigurations :
    IEntityTypeConfiguration<ResumeCertificate>
{
    public void Configure(EntityTypeBuilder<ResumeCertificate> builder)
    {
        builder.ToTable("ResumeCertificates", "Resumes");

        builder.HasKey(rc => rc.Id);
        builder.HasIndex(rc => rc.ResumeId);

        builder.Property(rc => rc.Title)
               .IsRequired()
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.Property(rc => rc.Issuer)
               .HasMaxLength(150)
               .HasColumnType("nvarchar(150)");

        builder.HasOne<Resume>()
               .WithMany(r => r.Certificates)
               .HasForeignKey(rc => rc.ResumeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
