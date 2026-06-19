using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Identity.SecuritySessions.Configurations;

public class SecuritySessionConfigurations : 
    IEntityTypeConfiguration<SecuritySession>
{
    public void Configure(EntityTypeBuilder<SecuritySession> builder)
    {
        builder.ToTable("SecuritySessions", "Identity");

        builder.HasKey(us => us.Id);
        builder.HasIndex(us => us.UserId);

        builder.Property(us => us.IpAddress)
               .IsRequired()
               .HasMaxLength(45)
               .HasColumnType("varchar(45)");

        builder.Property(us => us.City)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");
        
        builder.Property(us => us.Country)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.HasOne<User>()
               .WithMany(u => u.Sessions)
               .HasForeignKey(us => us.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ss => ss.Device)
               .WithMany(d => d.Sessions)
               .HasForeignKey(ss => ss.DeviceId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.HasMany(ss => ss.RefreshTokens)
               .WithOne()
               .HasForeignKey(rt => rt.SecuritySessionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
