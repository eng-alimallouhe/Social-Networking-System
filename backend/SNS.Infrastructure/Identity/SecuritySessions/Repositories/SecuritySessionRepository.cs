using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity.SecuritySessions.Repositories;

public class SecuritySessionRepository : Repository<SecuritySession>
{
    public SecuritySessionRepository(SNSDbContext context) : base(context) { }
}
