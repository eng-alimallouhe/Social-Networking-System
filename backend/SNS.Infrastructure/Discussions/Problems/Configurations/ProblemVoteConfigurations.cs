using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Discussions.Problems.Entities;

namespace SNS.Infrastructure.Discussions.Problems.Configurations;

public class ProblemVoteConfigurations : 
    IEntityTypeConfiguration<ProblemVote>
{
    public void Configure(EntityTypeBuilder<ProblemVote> builder)
    {
        builder.ToTable("ProblemVotes", "QA");

        builder.HasKey(pv => pv.Id);

        builder.HasIndex(pv => pv.ProblemId);

        builder.HasIndex(pv => pv.VoterId);

        // Composite Unique Index: User can only vote once per problem
        builder.HasIndex(
            pv => new 
            { 
                pv.ProblemId, 
                pv.VoterId 
            }).IsUnique();

        builder.Property(pv => pv.Type)
               .HasConversion<int>();

        builder.HasOne<Problem>()
               .WithMany(p => p.Votes)
               .HasForeignKey(pv => pv.ProblemId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(pv => pv.VoterId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
