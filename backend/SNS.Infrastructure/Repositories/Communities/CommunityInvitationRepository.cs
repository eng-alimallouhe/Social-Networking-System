using SNS.Domain.Communities.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Communities
{
    public class CommunityInvitationRepository : Repository<CommunityInvitation>
    {
        public CommunityInvitationRepository(SNSDbContext context) : base(context) { }
    }
}
