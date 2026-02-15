using SNS.Domain.Security.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class NotificationRepository : Repository<Notification>
{
    public NotificationRepository(SNSDbContext context) : base(context) { }
}
