using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.QA.Entities; 

namespace SNS.Infrastructure.Configurations.QA;

public class ProblemVoteConfigurations : 
    IEntityTypeConfiguration<ProblemVote>
{
    public void Configure(EntityTypeBuilder<ProblemVote> builder)
    {
        builder.ToTable("ProblemVotes");

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

        builder.HasOne(pv => pv.Problem)
               .WithMany(p => p.Votes)
               .HasForeignKey(pv => pv.ProblemId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pv => pv.Voter)
               .WithMany(p => p.ProblemVotes)
               .HasForeignKey(pv => pv.VoterId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}