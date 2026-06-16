using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.ContentManagement.Posts.Entities;

namespace SNS.Infrastructure.ContentManagement.Posts.Configurations;

public class PostViewConfigurations :
    IEntityTypeConfiguration<PostView>
{
    public void Configure(EntityTypeBuilder<PostView> builder)
    {
        builder.ToTable("PostViews", "ContentManagement");

        builder.HasKey(pv => pv.Id);

        // Standard FK Indexes
        builder.HasIndex(pv => pv.PostId);
        builder.HasIndex(pv => pv.ViewerId);

        // -------------------------------------------------------------
        // CONDITIONAL UNIQUE INDEX
        // -------------------------------------------------------------
        // Prevents duplicate ACTIVE views. Allows history of inactive views.
        builder.HasIndex(
            pv => new 
            { 
                pv.PostId, 
                pv.ViewerId 
            })
            .IsUnique()
            .HasFilter("[IsActive] = 1");
        // -------------------------------------------------------------

        builder.Property(pv => pv.IpHash)
               .HasMaxLength(128)
               .HasColumnType("varchar(128)");

        builder.Property(pv => pv.Country)
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(pv => pv.DeviceType).HasConversion<int>();

        builder.HasOne<Post>()
               .WithMany(p => p.Views)
               .HasForeignKey(pv => pv.PostId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Profile>()
               .WithMany() 
               .HasForeignKey(pv => pv.ViewerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
