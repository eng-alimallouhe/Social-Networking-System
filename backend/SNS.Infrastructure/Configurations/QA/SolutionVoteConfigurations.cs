using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.QA.Bridges; // For SolutionVote

namespace SNS.Infrastructure.Configurations.QA;

public class SolutionVoteConfigurations : 
    IEntityTypeConfiguration<SolutionVote>
{
    public void Configure(EntityTypeBuilder<SolutionVote> builder)
    {
        builder.ToTable("SolutionVotes");

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

        builder.HasOne(sv => sv.Solution)
               .WithMany(s => s.Votes)
               .HasForeignKey(sv => sv.SolutionId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sv => sv.Voter)
               .WithMany(p => p.SolutionVotes)
               .HasForeignKey(sv => sv.VoterId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}