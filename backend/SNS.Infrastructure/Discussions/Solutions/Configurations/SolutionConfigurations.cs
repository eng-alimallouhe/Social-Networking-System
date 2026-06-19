using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.Discussions.Solutions.Configurations;

public class SolutionConfigurations : 
    IEntityTypeConfiguration<Solution>
{
    public void Configure(EntityTypeBuilder<Solution> builder)
    {
        builder.ToTable("Solutions", "QA");

        builder.HasKey(s => s.Id);

        builder.HasIndex(s => s.ProblemId);
        builder.HasIndex(s => s.AuthorId);

        builder.Property(s => s.Status)
               .HasConversion<int>();

        builder.HasOne<Problem>()
               .WithMany(p => p.Solutions)
               .HasForeignKey(s => s.ProblemId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Profile>()
               .WithMany() 
               .HasForeignKey(s => s.AuthorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
