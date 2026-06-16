using Microsoft.EntityFrameworkCore;
using SNS.Domain.Identity.Notifications.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity;

public class UserNotificationPreferencesRepository : Repository<UserNotificationPreferences>
{
    public UserNotificationPreferencesRepository(SNSDbContext context) : base(context)
    {
    }
}
