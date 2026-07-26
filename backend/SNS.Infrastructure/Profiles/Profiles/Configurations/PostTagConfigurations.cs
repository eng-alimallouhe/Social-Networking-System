using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Profiles.Profiles.Relations;

namespace SNS.Infrastructure.Profiles.Profiles.Configurations;

public class PostTagConfigurations
    : IEntityTypeConfiguration<ProfileTag>
{
    public void Configure(EntityTypeBuilder<ProfileTag> builder)
    {
        builder.ToTable("ProfileTags", "Profiles");
        
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.ProfileId,
            x.TagId
        }).IsUnique();


        builder.HasOne(x => x.Tag)
            .WithMany()
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Profile>()
            .WithMany(p => p.ProfileTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}