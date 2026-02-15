using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ProfileContext.Bridges;

namespace SNS.Infrastructure.Configurations.ProfileContext;

public class ProfileSkillRequestConfigurations :
    IEntityTypeConfiguration<ProfileSkillRequest>
{
    public void Configure(EntityTypeBuilder<ProfileSkillRequest> builder)
    {
        builder.ToTable("ProfileSkillRequests");

        builder.HasKey(psr => psr.Id);

        builder.HasIndex(psr => psr.JoinerId);
        builder.HasIndex(psr => psr.SkillRequestId);

        // Unique Constraint
        builder.HasIndex(
            psr => new 
            { 
                psr.JoinerId, 
                psr.SkillRequestId 
            })
            .IsUnique();

        builder.Property(psr => psr.Level)
               .HasConversion<int>();

        builder.HasOne(psr => psr.Joiner)
               .WithMany()
               .HasForeignKey(psr => psr.JoinerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(psr => psr.SkillRequest)
               .WithMany(sr => sr.ProfileSkillRequests)
               .HasForeignKey(psr => psr.SkillRequestId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}