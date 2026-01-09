using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Content.Entities;

namespace SNS.Infrastructure.Configurations.Posts;

public class PostReactionConfigurations :
    IEntityTypeConfiguration<PostReaction>
{
    public void Configure(EntityTypeBuilder<PostReaction> builder)
    {
        builder.ToTable("PostReactions");

        builder.HasKey(pr => pr.Id);
        builder.HasIndex(pr => pr.PostId);
        builder.HasIndex(pr => pr.ReactorId);

        // Unique Constraint: One reaction per user per post
        builder.HasIndex(
            pr => new 
            { 
                pr.PostId, 
                pr.ReactorId 
            })
            .IsUnique();

        builder.Property(pr => pr.Type).HasConversion<int>();

        builder.HasOne(pr => pr.Post)
               .WithMany(p => p.Reactions)
               .HasForeignKey(pr => pr.PostId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pr => pr.Reactor)
               .WithMany(p => p.PostReactions)
               .HasForeignKey(pr => pr.ReactorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}