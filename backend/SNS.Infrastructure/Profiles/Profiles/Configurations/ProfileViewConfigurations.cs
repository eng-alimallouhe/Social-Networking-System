using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.Profiles.Profiles.Configurations;

public class ProfileViewConfigurations : 
    IEntityTypeConfiguration<ProfileView>
{
    public void Configure(EntityTypeBuilder<ProfileView> builder)
    {
        builder.ToTable("ProfileViews", "Profiles");

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

        builder.HasOne<Profile>(pv => pv.Viewer)
               .WithMany(p => p.Views) 
               .HasForeignKey(pv => pv.ViewerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Profile>(pv => pv.Viewed)
               .WithMany(p => p.Vieweds) 
               .HasForeignKey(pv => pv.ViewedId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
