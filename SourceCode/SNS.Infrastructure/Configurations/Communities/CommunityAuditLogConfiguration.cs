using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Communities.Entities;

namespace SNS.Infrastructure.Configurations.Communities;

public class CommunityAuditLogConfigurations :
    IEntityTypeConfiguration<CommunityAuditLog>
{
    public void Configure(EntityTypeBuilder<CommunityAuditLog> builder)
    {
        builder.ToTable("CommunityAuditLogs");

        builder.HasKey(cal => cal.Id);
        builder.HasIndex(cal => cal.CommunityId);
        builder.HasIndex(cal => cal.ActorId);

        builder.Property(cal => cal.Action)
               .IsRequired()
               .HasMaxLength(255)
               .HasColumnType("nvarchar(255)");

        builder.HasOne(cal => cal.Community)
               .WithMany(c => c.AuditLogs)
               .HasForeignKey(cal => cal.CommunityId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cal => cal.Actor)
               .WithMany()
               .HasForeignKey(cal => cal.ActorId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}