using SNS.Domain.Identity.Notifications.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity;

public class NotificationRepository : Repository<Notification>
{
    public NotificationRepository(SNSDbContext context) : base(context) { }
}
