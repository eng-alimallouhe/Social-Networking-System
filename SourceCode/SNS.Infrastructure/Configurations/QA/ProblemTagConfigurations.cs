using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.QA.Bridges;

namespace SNS.Infrastructure.Configurations.QA
{
    public class ProblemTagConfigurations : IEntityTypeConfiguration<ProblemTag>
    {
        public void Configure(EntityTypeBuilder<ProblemTag> builder)
        {
            builder.ToTable("ProblemTags");

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

            builder.HasOne(pt => pt.Problem)
                   .WithMany(p => p.Tags)
                   .HasForeignKey(pt => pt.ProblemId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pt => pt.Tag)
                   .WithMany() // Assuming Tag entity exists
                   .HasForeignKey(pt => pt.TagId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}