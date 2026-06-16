using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.Discussions.Problems.Configurations;

public class ProblemViewConfigurations : 
    IEntityTypeConfiguration<ProblemView>
{
    public void Configure(EntityTypeBuilder<ProblemView> builder)
    {
        builder.ToTable("ProblemViews", "QA");

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

        builder.HasOne<Problem>()
               .WithMany(p => p.Views)
               .HasForeignKey(pv => pv.ProblemId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(pv => pv.ViewerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
