using SNS.Domain.Security.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class UserSessionRepository : Repository<UserSession>
{
    public UserSessionRepository(SNSDbContext context) : base(context) { }
}
