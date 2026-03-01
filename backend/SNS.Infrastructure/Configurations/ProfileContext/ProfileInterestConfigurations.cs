using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ProfileContext.Bridges;

namespace SNS.Infrastructure.Configurations.ProfileContext;

public class ProfileInterestConfigurations :
    IEntityTypeConfiguration<ProfileInterest>
{
    public void Configure(EntityTypeBuilder<ProfileInterest> builder)
    {
        builder.ToTable("ProfileInterests");

        builder.HasKey(pi => pi.Id);

        // Foreign Key Indexes
        builder.HasIndex(pi => pi.ProfileId);
        builder.HasIndex(pi => pi.InterestId);

        // Unique Constraint: Prevent duplicate interest entries for the same profile
        builder.HasIndex(
            pi => new 
            { 
                pi.ProfileId, 
                pi.InterestId 
            })
            .IsUnique();

        // Relationships
        builder.HasOne<SNS.Domain.SocialGraph.Profile>()
               .WithMany(p => p.ProfileInterests)
               .HasForeignKey(pi => pi.ProfileId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<SNS.Domain.Preferences.Entities.Interest>()
               .WithMany()
               .HasForeignKey(pi => pi.InterestId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
