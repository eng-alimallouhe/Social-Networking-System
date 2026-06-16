using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.ContentManagement.Communities.Repositories
{
    public class CommunityMembershipRepository : Repository<CommunityMembership>
    {
        public CommunityMembershipRepository(SNSDbContext context) : base(context) { }
    }
}
