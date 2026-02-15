using SNS.Domain.Communities.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Communities
{
    public class CommunityMembershipRepository : Repository<CommunityMembership>
    {
        public CommunityMembershipRepository(SNSDbContext context) : base(context) { }
    }
}
