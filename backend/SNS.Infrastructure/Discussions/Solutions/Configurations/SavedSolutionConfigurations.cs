using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Discussions.Solutions.Relations;

namespace SNS.Infrastructure.Discussions.Solutions.Configurations;

public class SavedSolutionConfigurations : IEntityTypeConfiguration<SavedSolution>
{
    public void Configure(EntityTypeBuilder<SavedSolution> builder)
    {
        builder.ToTable("SavedSolutions", "QA");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ProfileId, x.SolutionId })
               .IsUnique();

        builder.HasIndex(x => new { x.ProfileId, x.SavedAt });

        builder.HasIndex(x => x.SolutionId);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(x => x.ProfileId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.Solution)
               .WithMany()
               .HasForeignKey(x => x.SolutionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
