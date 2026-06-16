using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Discussions.Solutions.Relations;
using SNS.Domain.Discussions.Solutions.Entities;

namespace SNS.Infrastructure.Discussions.Solutions.Configurations;

public class SolutionVoteConfigurations : 
    IEntityTypeConfiguration<SolutionVote>
{
    public void Configure(EntityTypeBuilder<SolutionVote> builder)
    {
        builder.ToTable("SolutionVotes", "QA");

        builder.HasKey(sv => sv.Id);
        builder.HasIndex(sv => sv.SolutionId);
        builder.HasIndex(sv => sv.VoterId);

        // Composite Unique Index
        builder.HasIndex(
            sv => new 
            { 
                sv.SolutionId, 
                sv.VoterId 
            }).IsUnique();

        builder.Property(sv => sv.Type)
               .HasConversion<int>();

        builder.HasOne<Solution>()
               .WithMany(s => s.Votes)
               .HasForeignKey(sv => sv.SolutionId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(sv => sv.VoterId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
