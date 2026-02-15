using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Security.Entities;

namespace SNS.Infrastructure.Configurations.Security;

public class UserSessionConfigurations : 
    IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");

        builder.HasKey(us => us.Id);
        builder.HasIndex(us => us.UserId);

        builder.Property(us => us.IpAddress)
               .IsRequired()
               .HasMaxLength(45)
               .HasColumnType("varchar(45)");

        builder.Property(us => us.Device)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(us => us.Browser)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.Property(us => us.Country)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("nvarchar(100)");

        builder.HasOne(us => us.User)
               .WithMany(u => u.Sessions)
               .HasForeignKey(us => us.UserId)
               .IsRequired();
    }
}