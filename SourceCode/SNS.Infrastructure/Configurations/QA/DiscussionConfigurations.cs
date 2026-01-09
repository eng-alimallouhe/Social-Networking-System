using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.QA.Entities;

namespace SNS.Infrastructure.Configurations.QA;

public class DiscussionConfigurations : 
    IEntityTypeConfiguration<Discussion>
{
    public void Configure(EntityTypeBuilder<Discussion> builder)
    {
        builder.ToTable("Discussions");

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
        builder.HasOne(d => d.Solution)
               .WithMany(s => s.Discussions)
               .HasForeignKey(d => d.SolutionId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Author)
               .WithMany(p => p.Discussions)
               .HasForeignKey(d => d.AuthorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing relationship (Replies)
        builder.HasOne(d => d.ParentDiscussion)
               .WithMany(p => p.Replies)
               .HasForeignKey(d => d.ParentDiscussionId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict); // Important: No Cascade on self-ref to avoid cycles
    }
}