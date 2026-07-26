using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.Profiles.Profiles.Entities;

namespace SNS.Infrastructure.ContentManagement.Communities.Configurations;

public class CommunityAuditLogConfigurations :
    IEntityTypeConfiguration<CommunityAuditLog>
{
    public void Configure(EntityTypeBuilder<CommunityAuditLog> builder)
    {
        builder.ToTable("CommunityAuditLogs", "Communities");

        builder.HasKey(cal => cal.Id);
        builder.HasIndex(cal => cal.CommunityId);
        builder.HasIndex(cal => cal.ActorId);

        builder.Property(cal => cal.Action)
               .IsRequired();

        builder.HasOne<Community>()
               .WithMany(c => c.AuditLogs)
               .HasForeignKey(cal => cal.CommunityId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(cal => cal.ActorId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
