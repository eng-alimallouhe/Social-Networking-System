using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ContentManagement.Posts.Entities;

namespace SNS.Infrastructure.ContentManagement.Posts.Configurations;

public class PostMentionConfigurations
    : IEntityTypeConfiguration<PostMention>
{
    public void Configure(EntityTypeBuilder<PostMention> builder)
    {
        builder.ToTable("PostMentions");

        builder.HasKey(cm => new
        {
            cm.ProfileId,
            cm.PostId
        });

        builder.HasOne(pm => pm.Profile)
            .WithMany()
            .HasForeignKey(pm => pm.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pm => pm.Post)
            .WithMany(p => p.Mentions)
            .HasForeignKey(pm => pm.PostId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
