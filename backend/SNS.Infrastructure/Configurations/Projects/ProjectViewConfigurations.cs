using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Projects.Bridges;

namespace SNS.Infrastructure.Configurations.Projects
{
    public class ProjectViewConfigurations : IEntityTypeConfiguration<ProjectView>
    {
        public void Configure(EntityTypeBuilder<ProjectView> builder)
        {
            builder.ToTable("ProjectViews");

            builder.HasKey(pv => pv.Id);

            // Standard FK Indexes
            builder.HasIndex(pv => pv.ProjectId);
            builder.HasIndex(pv => pv.ViewerId);

            // -------------------------------------------------------------
            // CONDITIONAL UNIQUE INDEX (The specific requirement)
            // -------------------------------------------------------------
            // Prevents duplicate active views, allows duplicate inactive views.
            builder.HasIndex(
                pv => new 
                { 
                    pv.ProjectId, 
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

            builder.Property(pv => pv.DeviceType)
                   .HasConversion<int>();

            builder.HasOne(pv => pv.Project)
                   .WithMany(p => p.Views)
                   .HasForeignKey(pv => pv.ProjectId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pv => pv.Viewer)
                   .WithMany(p => p.ProjectViews)
                   .HasForeignKey(pv => pv.ViewerId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}