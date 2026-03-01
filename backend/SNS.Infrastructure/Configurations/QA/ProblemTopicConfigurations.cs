using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.QA.Bridges;

namespace SNS.Infrastructure.Configurations.QA
{
    public class ProblemTopicConfigurations : IEntityTypeConfiguration<ProblemTopic>
    {
        public void Configure(EntityTypeBuilder<ProblemTopic> builder)
        {
            builder.ToTable("ProblemTopics");

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

            builder.HasOne<SNS.Domain.QA.Entities.Problem>()
                   .WithMany()
                   .HasForeignKey(pt => pt.ProblemId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<SNS.Domain.Preferences.Entities.Topic>()
                   .WithMany() // Assuming Topic entity exists
                   .HasForeignKey(pt => pt.TopicId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}