using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Security.Entities;

namespace SNS.Infrastructure.Configurations.Security;

public class ManualRecoveryRequestConfigurations :
    IEntityTypeConfiguration<ManualRecoveryRequest>
{
    public void Configure(EntityTypeBuilder<ManualRecoveryRequest> builder)
    {
        builder.ToTable("ManualRecoveryRequests");

        builder.HasKey(mr => mr.Id);
        builder.HasIndex(mr => mr.SubmitterId);
        builder.HasIndex(mr => mr.ReviewerId);

        builder.Property(mr => mr.ContactEmail)
               .IsRequired()
               .HasMaxLength(255)
               .HasColumnType("varchar(255)");

        builder.Property(mr => mr.ContactPhone)
               .HasMaxLength(20)
               .HasColumnType("varchar(20)");

        builder.Property(mr => mr.ProvidedInfoJson)
               .HasColumnType("nvarchar(max)");

        builder.Property(mr => mr.ReviewerNotes)
               .HasMaxLength(1000)
               .HasColumnType("nvarchar(1000)");

        builder.HasOne(mr => mr.Submitter)
               .WithMany(u => u.RecoveryRequests)
               .HasForeignKey(mr => mr.SubmitterId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mr => mr.Reviewer)
               .WithMany(u => u.RecoveryReviews)
               .HasForeignKey(mr => mr.ReviewerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
    }
}