using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.ContentManagement.Comments.Configurations;

public class CommentReactionConfigurations :
    IEntityTypeConfiguration<CommentReaction>
{
    public void Configure(EntityTypeBuilder<CommentReaction> builder)
    {
        builder.ToTable("CommentReactions", "ContentManagement");

        builder.HasKey(cr => cr.Id);
        builder.HasIndex(cr => cr.CommentId);
        builder.HasIndex(cr => cr.ReactorId);

        // Unique Constraint: One reaction per user per comment
        builder.HasIndex(
            cr => new 
            { 
                cr.CommentId, 
                cr.ReactorId 
            })
            .IsUnique();

        builder.Property(cr => cr.Type).HasConversion<int>();

        builder.HasOne<Comment>()
               .WithMany(c => c.Reactions)
               .HasForeignKey(cr => cr.CommentId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(cr => cr.ReactorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}