using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.ContentManagement.Communities.Configurations;

public class CommunityJoinRequestConfiguration : IEntityTypeConfiguration<CommunityJoinRequest>
{
    public void Configure(EntityTypeBuilder<CommunityJoinRequest> builder)
    {
        builder.ToTable("CommunityJoinRequests", "Communities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.HasIndex(x => new 
        { 
            x.CommunityId, 
            x.SubmitterId 
        }).IsUnique();

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

