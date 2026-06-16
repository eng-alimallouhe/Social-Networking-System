using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.Notifications.Entities;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Identity.Notifications.Configurations;

public class NotificationConfigurations : 
    IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications", "Identity");

        builder.HasKey(n => n.Id);
        builder.HasIndex(n => n.UserId);

        builder.HasIndex(n => n.TargetId);

        builder.HasOne<User>()
               .WithMany(u => u.Notifications)
               .HasForeignKey(n => n.UserId)
               .IsRequired();
    }
}
