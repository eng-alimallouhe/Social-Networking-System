using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ContentManagement.Comments.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNS.Infrastructure.ContentManagement.Comments.Configurations;

public class CommentMentionConifurations
    : IEntityTypeConfiguration<CommentMention>
{
    public void Configure(EntityTypeBuilder<CommentMention> builder)
    {
        builder.ToTable("CommentMentions");

        builder.HasKey(cm => new
        {
            cm.ProfileId,
            cm.CommentId
        });

        builder.HasOne(cm => cm.Profile)
            .WithMany()
            .HasForeignKey(cm => cm.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cm => cm.Comment)
            .WithMany(p => p.Mentions)
            .HasForeignKey(pm => pm.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}