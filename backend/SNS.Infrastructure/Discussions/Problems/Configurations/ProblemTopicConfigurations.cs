using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Preferences.Entities;

namespace SNS.Infrastructure.Discussions.Problems.Configurations
{
    public class ProblemTopicConfigurations : IEntityTypeConfiguration<ProblemTopic>
    {
        public void Configure(EntityTypeBuilder<ProblemTopic> builder)
        {
            builder.ToTable("ProblemTopics", "QA");

            builder.HasKey(pt => pt.Id);

            builder.HasIndex(
                pt => new 
                { 
                    pt.ProblemId, 
                    pt.TopicId 
                })
                .IsUnique();

            builder.Property(pt => pt.Confidence)
                   .HasColumnType("real"); 

            builder.HasOne<Problem>()
                   .WithMany()
                   .HasForeignKey(pt => pt.ProblemId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Topic>()
                   .WithMany()
                   .HasForeignKey(pt => pt.TopicId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
