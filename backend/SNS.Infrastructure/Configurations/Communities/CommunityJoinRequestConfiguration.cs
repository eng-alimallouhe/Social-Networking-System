using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Communities.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Infrastructure.Configurations.Communities;

public class CommunityJoinRequestConfiguration : IEntityTypeConfiguration<CommunityJoinRequest>
{
    public void Configure(EntityTypeBuilder<CommunityJoinRequest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.HasOne<Community>()
            .WithMany()
            .HasForeignKey(x => x.CommunityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Profile>()
            .WithMany()
            .HasForeignKey(x => x.SubmitterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

