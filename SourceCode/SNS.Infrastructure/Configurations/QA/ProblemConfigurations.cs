using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Communities.Entities;
using SNS.Domain.QA.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Infrastructure.Configurations.QA
{
    public class ProblemConfigurations : IEntityTypeConfiguration<Problem>
    {
        public void Configure(EntityTypeBuilder<Problem> builder)
        {
            builder.ToTable("Problems");

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

            builder.Property(p => p.ReadmeContent)
                   .HasColumnType("nvarchar(max)"); // Markdown content can be large

            builder.Property(p => p.Status)
                   .HasConversion<int>();

            builder.Property(p => p.Level)
                   .HasConversion<int>();

            // Relationships
            builder.HasOne(p => p.Author)
                   .WithMany(profile => profile.Problems) 
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