using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Education.Entities;

namespace SNS.Infrastructure.Configurations.ProfileContext;

public class ProfileUniversityRequestConfiguration :
    IEntityTypeConfiguration<ProfileUniversityRequest>
{
    public void Configure(EntityTypeBuilder<ProfileUniversityRequest> builder)
    {
        builder.ToTable("ProfileUniversityRequests");

        builder.HasKey(pur => pur.Id);

        builder.HasIndex(pur => pur.JoinerId);
        builder.HasIndex(pur => pur.UniversityRequestId);

        // Unique Constraint: A profile can support a university request only once
        builder.HasIndex(pur => new { pur.JoinerId, pur.UniversityRequestId }).IsUnique();

        builder.HasOne(pur => pur.Joiner)
               .WithMany()
               .HasForeignKey(pur => pur.JoinerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pur => pur.UniversityRequest)
               .WithMany(ur => ur.Requests)
               .HasForeignKey(pur => pur.UniversityRequestId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}