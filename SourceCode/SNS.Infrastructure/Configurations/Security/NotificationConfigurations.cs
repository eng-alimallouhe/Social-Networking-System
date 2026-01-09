using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Security.Entities;

namespace SNS.Infrastructure.Configurations.Security;

public class NotificationConfigurations : 
    IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);
        builder.HasIndex(n => n.UserId);

        builder.HasIndex(n => n.TargetId);

        builder.HasOne<User>()
               .WithMany(u => u.Notification)
               .HasForeignKey(n => n.UserId)
               .IsRequired();
    }
}