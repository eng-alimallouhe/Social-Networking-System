using SNS.Domain.Communities.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Communities
{
    public class CommunityCreationRequestRepository : Repository<CommunityCreationRequest>
    {
        public CommunityCreationRequestRepository(SNSDbContext context) : base(context) { }
    }
}
