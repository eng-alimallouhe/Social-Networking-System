using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.ContentManagement.Posts.Configurations;

public class PostReactionConfigurations :
    IEntityTypeConfiguration<PostReaction>
{
    public void Configure(EntityTypeBuilder<PostReaction> builder)
    {
        builder.ToTable("PostReactions", "ContentManagement");

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

        builder.HasOne<Post>()
               .WithMany(p => p.Reactions)
               .HasForeignKey(pr => pr.PostId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(pr => pr.ReactorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
