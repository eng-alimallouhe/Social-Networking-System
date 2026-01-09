using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Content.Entities;

namespace SNS.Infrastructure.Configurations.Posts;

public class PostConfigurations : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.AuthorId);

        builder.HasIndex(p => p.CommunityId);

        builder.HasIndex(p => p.Title);

        builder.Property(p => p.Title)
               .IsRequired()
               .HasMaxLength(255)
               .HasColumnType("nvarchar(255)");

        builder.Property(p => p.Content)
               .HasColumnType("nvarchar(max)"); 

        // Enums
        builder.Property(p => p.Type).HasConversion<int>();
        builder.Property(p => p.Status).HasConversion<int>();

        // Relationships
        builder.HasOne(p => p.Author)
               .WithMany(profile => profile.Posts)
               .HasForeignKey(p => p.AuthorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Community)
               .WithMany(c => c.Posts) 
               .HasForeignKey(p => p.CommunityId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}