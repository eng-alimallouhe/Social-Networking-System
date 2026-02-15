using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.SocialGraph.Bridges;

namespace SNS.Infrastructure.Configurations.SocialGraph;

public class FollowConfigurations : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable("Follows");

        builder.HasKey(f => f.Id);

        // FK Indexes
        builder.HasIndex(f => f.FollowerId);
        builder.HasIndex(f => f.FollowingId);

        // Unique Constraint: User A cannot follow User B more than once
        builder.HasIndex(
            f => new 
            { 
                f.FollowerId, 
                f.FollowingId 
            }).IsUnique();

        // Relationships

        // Follower side
        builder.HasOne(f => f.Follower)
               .WithMany(p => p.Followings)
               .HasForeignKey(f => f.FollowerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict); // Prevent Cascade Cycles

        // Following side
        builder.HasOne(f => f.Following)
               .WithMany(p => p.Followers)
               .HasForeignKey(f => f.FollowingId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict); // Prevent Cascade Cycles
    }
}