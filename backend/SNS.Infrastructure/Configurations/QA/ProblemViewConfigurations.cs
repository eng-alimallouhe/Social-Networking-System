using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.QA.Bridges;

namespace SNS.Infrastructure.Configurations.QA;

public class ProblemViewConfigurations : 
    IEntityTypeConfiguration<ProblemView>
{
    public void Configure(EntityTypeBuilder<ProblemView> builder)
    {
        builder.ToTable("ProblemViews");

        builder.HasKey(pv => pv.Id);

        builder.HasIndex(pv => pv.ProblemId);

        builder.HasIndex(pv => pv.ViewerId);

        builder.HasIndex(
            pv => new
            {
                pv.ProblemId,
                pv.ViewerId,
            })
            .IsUnique();

        builder.Property(pv => pv.IpHash)
               .HasMaxLength(128)
               .HasColumnType("varchar(128)");

        builder.Property(pv => pv.Country)
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(pv => pv.DeviceType)
               .HasConversion<int>();

        builder.HasOne(pv => pv.Problem)
               .WithMany(p => p.Views)
               .HasForeignKey(pv => pv.ProblemId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pv => pv.Viewer)
               .WithMany(p => p.ProblemViews)
               .HasForeignKey(pv => pv.ViewerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}