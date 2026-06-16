using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.ContentManagement.Communities.Repositories
{
   public class CommunityAuditLogRepository : Repository<CommunityAuditLog>
    {
        public CommunityAuditLogRepository(SNSDbContext context) : base(context) { }
    }
}
