using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Posts.Bridges;

namespace SNS.Infrastructure.Configurations.Posts;

public class PostViewConfigurations :
    IEntityTypeConfiguration<PostView>
{
    public void Configure(EntityTypeBuilder<PostView> builder)
    {
        builder.ToTable("PostViews");

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

        builder.HasOne(pv => pv.Post)
               .WithMany(p => p.Views)
               .HasForeignKey(pv => pv.PostId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pv => pv.Viewer)
               .WithMany(p => p.PostViews) 
               .HasForeignKey(pv => pv.ViewerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}