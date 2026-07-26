using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.Profiles.SocialGraph.Configurations;

public class BlockConfigurations : 
    IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.ToTable("Blocks", "Profiles");

        builder.HasKey(b => b.Id);

        // FK Indexes
        builder.HasIndex(b => b.BlockerId);
        builder.HasIndex(b => b.BlockedId);

        // Unique Constraint: User A cannot block User B more than once
        builder.HasIndex(
            b => new 
            { 
                b.BlockerId, 
                b.BlockedId 
            }).IsUnique();

        // Relationships

        builder.HasOne<Profile>(b => b.Blocker)
               .WithMany(p => p.BlackList)
               .HasForeignKey(b => b.BlockerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Profile>(b => b.Blocked)
               .WithMany()
               .HasForeignKey(b => b.BlockedId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
