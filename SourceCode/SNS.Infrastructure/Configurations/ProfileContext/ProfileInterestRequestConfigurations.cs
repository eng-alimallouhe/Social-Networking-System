using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ProfileContext.Bridges;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Configurations.ProfileContext;

public class ProfileInterestRequestConfigurations :
    IEntityTypeConfiguration<ProfileInterestRequest>
{
    public void Configure(EntityTypeBuilder<ProfileInterestRequest> builder)
    {
        builder.ToTable("ProfileInterestRequests");

        builder.HasKey(pir => pir.Id);

        builder.HasIndex(pir => pir.JoinerId);
        builder.HasIndex(pir => pir.InterestRequestId);

        // Unique Constraint
        builder.HasIndex(
            pir => new 
            { 
                pir.JoinerId, 
                pir.InterestRequestId 
            }).IsUnique();

        builder.HasOne(pir => pir.Joiner)
               .WithMany()
               .HasForeignKey(pir => pir.JoinerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pir => pir.InterestRequest)
               .WithMany(ir => ir.ProfileInterestRequests)
               .HasForeignKey(pir => pir.InterestRequestId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}