using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Identity.SecuritySessions.Configurations;

public class DeviceConfigurations : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices", "Identity");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.DeviceToken)
            .IsUnique();

        builder.Property(us => us.DeviceToken)
               .IsRequired()
               .HasMaxLength(64)
               .HasColumnType("nvarchar(64)");

        builder.Property(us => us.FriendlyName)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(us => us.Browser)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(us => us.OperatingSystem)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(us => us.DeviceVendor)
               .IsRequired(false)
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(us => us.DeviceModel)
               .IsRequired(false)
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(us => us.FingerprintHash)
               .IsRequired()
               .HasMaxLength(64)
               .HasColumnType("nvarchar(64)");
        
        builder.Property(us => us.IsTrusted)
               .IsRequired()
               .HasColumnType("bit");
        
        builder.Property(us => us.FirstSeenAt)
               .IsRequired()
               .HasColumnType("DATETIME");
        
        builder.Property(us => us.LastSeenAt)
               .IsRequired()
               .HasColumnType("DATETIME");


        builder.HasOne<User>()
            .WithMany(u => u.Devices)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
