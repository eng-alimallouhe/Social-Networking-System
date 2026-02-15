using SNS.Domain.Communities.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Communities
{
   public class CommunityAuditLogRepository : Repository<CommunityAuditLog>
    {
        public CommunityAuditLogRepository(SNSDbContext context) : base(context) { }
    }
}
