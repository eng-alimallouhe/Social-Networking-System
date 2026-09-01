using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.ContentManagement.Comments.Configurations;

public class CommentConfigurations : 
    IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments", "ContentManagement");

        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.PostId);
        builder.HasIndex(c => c.AuthorId);
        builder.HasIndex(c => c.ParentCommentId);

        builder.Property(c => c.Content)
               .IsRequired()
               .HasMaxLength(2000) 
               .HasColumnType("nvarchar(2000)");

        // Relationships
        builder.HasOne<Post>()
               .WithMany(p => p.Comments)
               .HasForeignKey(c => c.PostId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); 


        builder.HasOne(c => c.Author)
               .WithMany()
               .HasForeignKey(c => c.AuthorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.NoAction);

        // Self-referencing (Replies)
        builder.HasOne(c => c.ParentComment)
               .WithMany(c => c.Replies)
               .HasForeignKey(c => c.ParentCommentId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.NoAction); // Avoid cycles
    }
}
