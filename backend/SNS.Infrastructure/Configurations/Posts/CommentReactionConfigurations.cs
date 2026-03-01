using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Content.Entities;
using SNS.Domain.Posts.Entities;

namespace SNS.Infrastructure.Configurations.Posts;

public class CommentReactionConfigurations :
    IEntityTypeConfiguration<CommentReaction>
{
    public void Configure(EntityTypeBuilder<CommentReaction> builder)
    {
        builder.ToTable("CommentReactions");

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

        builder.HasOne<SNS.Domain.Content.Entities.Comment>()
               .WithMany()
               .HasForeignKey(cr => cr.CommentId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<SNS.Domain.SocialGraph.Profile>()
               .WithMany()
               .HasForeignKey(cr => cr.ReactorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
