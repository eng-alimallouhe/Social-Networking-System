using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Discussions.Problems.Relations;

namespace SNS.Infrastructure.Discussions.Problems.Configurations;

public class SavedProblemConfigurations : IEntityTypeConfiguration<SavedProblem>
{
    public void Configure(EntityTypeBuilder<SavedProblem> builder)
    {
        builder.ToTable("SavedProblems", "QA");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ProfileId, x.ProblemId })
               .IsUnique();

        builder.HasIndex(x => new { x.ProfileId, x.SavedAt });

        builder.HasIndex(x => x.ProblemId);

        builder.HasOne<Profile>() 
               .WithMany()
               .HasForeignKey(x => x.ProfileId)
               .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(sp => sp.Problem)
               .WithMany()
               .HasForeignKey(x => x.ProblemId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
