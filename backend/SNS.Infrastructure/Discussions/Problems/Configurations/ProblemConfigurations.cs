using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.Discussions.Problems.Configurations
{
    public class ProblemConfigurations : IEntityTypeConfiguration<Problem>
    {
        public void Configure(EntityTypeBuilder<Problem> builder)
        {
            builder.ToTable("Problems", "QA");

            builder.HasKey(p => p.Id);

            // Indexes
            builder.HasIndex(p => p.AuthorId);
            builder.HasIndex(p => p.CommunityId);

            // Search Index
            builder.HasIndex(p => p.Title);

            // Properties
            builder.Property(p => p.Title)
                   .IsRequired()
                   .HasMaxLength(255)
                   .HasColumnType("nvarchar(255)");


            builder.Property(p => p.Status)
                   .HasConversion<int>();

            builder.Property(p => p.Level)
                   .HasConversion<int>();

            // Relationships
            builder.HasOne<Profile>(p => p.Author)
                   .WithMany(p => p.Problems)
                   .HasForeignKey(p => p.AuthorId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Community)
                   .WithMany(c => c.Problems)
                   .HasForeignKey(p => p.CommunityId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
