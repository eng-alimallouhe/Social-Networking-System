using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Discussions.Problems.Configurations
{
    public class ProblemTagConfigurations : IEntityTypeConfiguration<ProblemTag>
    {
        public void Configure(EntityTypeBuilder<ProblemTag> builder)
        {
            builder.ToTable("ProblemTags", "QA");

            builder.HasKey(pt => pt.Id);

            builder.HasIndex(
                pt => new
                {
                    pt.ProblemId,
                    pt.TagId
                })
                .IsUnique();

            builder.HasIndex(pt => pt.ProblemId);

            builder.HasIndex(pt => pt.TagId);

            builder.HasOne<Problem>()
                   .WithMany()
                   .HasForeignKey(pt => pt.ProblemId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Tag>()
                   .WithMany() 
                   .HasForeignKey(pt => pt.TagId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
