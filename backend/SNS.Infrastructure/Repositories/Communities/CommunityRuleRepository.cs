using SNS.Domain.Communities.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Communities
{
    public class CommunityRuleRepository : Repository<CommunityRule>
    {
        public CommunityRuleRepository(SNSDbContext context) : base(context) { }
    }
}
