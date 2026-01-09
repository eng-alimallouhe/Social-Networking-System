using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.QA.Entities;

namespace SNS.Infrastructure.Configurations.QA;

public class SolutionConfigurations : 
    IEntityTypeConfiguration<Solution>
{
    public void Configure(EntityTypeBuilder<Solution> builder)
    {
        builder.ToTable("Solutions");

        builder.HasKey(s => s.Id);

        builder.HasIndex(s => s.ProblemId);
        builder.HasIndex(s => s.AuthorId);

        builder.Property(s => s.Status)
               .HasConversion<int>();

        builder.HasOne(s => s.Problem)
               .WithMany(p => p.Solutions)
               .HasForeignKey(s => s.ProblemId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Author)
               .WithMany(p => p.Solutions) 
               .HasForeignKey(s => s.AuthorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}