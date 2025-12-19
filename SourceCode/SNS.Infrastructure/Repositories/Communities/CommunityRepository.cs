using SNS.Domain.Communities.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Communities
{
    public class CommunityRepository : SoftDeletableRepository<Community>
    {
        public CommunityRepository(SNSDbContext context) : base(context) { }
    }
}
