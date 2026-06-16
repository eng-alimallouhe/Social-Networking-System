using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SNS.Domain.Identity.Notifications.Entities;
using SNS.Domain.Identity.Users.Entities;

namespace SNS.Infrastructure.Identity.Notifications.Configurations;

public class UserNotificationPreferencesConfigurations : IEntityTypeConfiguration<UserNotificationPreferences>
{
    public void Configure(EntityTypeBuilder<UserNotificationPreferences> builder)
    {
        builder.ToTable("UserNotificationPreferences", "Identity");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.UserId);


        builder.HasOne<User>()
            .WithOne(u => u.NotificationPreferences)
            .HasForeignKey<UserNotificationPreferences>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
