using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.ContentManagement.Posts.Entities;

namespace SNS.Infrastructure.ContentManagement.Posts.Configurations;

public class SavedPostConfigurations : IEntityTypeConfiguration<SavedPost>
{
    public void Configure(EntityTypeBuilder<SavedPost> builder)
    {
        builder.ToTable("SavedPosts", "ContentManagement");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProfileId)
            .IsRequired();

        builder.Property(sp => sp.PostId)
            .IsRequired();

        builder.HasIndex(sp => new 
        { 
            sp.ProfileId, 
            sp.PostId 
        }).IsUnique();

        builder.HasOne<Post>()
            .WithMany(p => p.SavedPosts)
            .HasForeignKey(sp => sp.PostId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne<Profile>()
            .WithMany()
            .HasForeignKey(sp => sp.ProfileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
