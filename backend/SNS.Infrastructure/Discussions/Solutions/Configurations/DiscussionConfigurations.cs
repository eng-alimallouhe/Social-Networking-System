using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.Discussions.Solutions.Configurations;

public class DiscussionConfigurations : 
    IEntityTypeConfiguration<Discussion>
{
    public void Configure(EntityTypeBuilder<Discussion> builder)
    {
        builder.ToTable("Discussions", "QA");

        builder.HasKey(d => d.Id);

        builder.HasIndex(d => d.SolutionId);
        builder.HasIndex(d => d.AuthorId);
        builder.HasIndex(d => d.ParentDiscussionId);

        builder.Property(d => d.Text)
               .HasColumnType("nvarchar(max)");

        builder.Property(d => d.Code)
               .HasColumnType("nvarchar(max)");

        builder.Property(d => d.CodeLanguage)
               .HasMaxLength(50)
               .HasColumnType("varchar(50)");

        // Relationships
        builder.HasOne<Solution>()
               .WithMany(s => s.Discussions)
               .HasForeignKey(d => d.SolutionId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(d => d.AuthorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing relationship (Replies)
        builder.HasOne<Discussion>()
               .WithMany(p => p.Replies)
               .HasForeignKey(d => d.ParentDiscussionId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict); // Important: No Cascade on self-ref to avoid cycles
    }
}
