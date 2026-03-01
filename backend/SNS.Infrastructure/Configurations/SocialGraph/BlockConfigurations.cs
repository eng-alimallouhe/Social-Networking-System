using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.SocialGraph.Bridges;

namespace SNS.Infrastructure.Configurations.SocialGraph;

public class BlockConfigurations : 
    IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.ToTable("Blocks");

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

        builder.HasOne<SNS.Domain.SocialGraph.Profile>()
               .WithMany()
               .HasForeignKey(b => b.BlockerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SNS.Domain.SocialGraph.Profile>()
               .WithMany()
               .HasForeignKey(b => b.BlockedId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}