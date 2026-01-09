using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.SocialGraph.Bridges;

namespace SNS.Infrastructure.Configurations.SocialGraph;

public class ProfileViewConfigurations : 
    IEntityTypeConfiguration<ProfileView>
{
    public void Configure(EntityTypeBuilder<ProfileView> builder)
    {
        builder.ToTable("ProfileViews");

        builder.HasKey(pv => pv.Id);

        builder.HasIndex(pv => pv.ViewerId);
        builder.HasIndex(pv => pv.ViewedId);

        builder.HasIndex(
        pv => new
        {
            pv.ViewedId,
            pv.ViewerId
        })
        .IsUnique()
        .HasFilter("[IsActive] = 1");

        builder.HasOne(pv => pv.Viewer)
               .WithMany(p => p.Vieweds) 
               .HasForeignKey(pv => pv.ViewerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pv => pv.Viewed)
               .WithMany(p => p.Views) 
               .HasForeignKey(pv => pv.ViewedId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}