using SNS.Domain.SocialGraph.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.SocialGraph;

public class FollowRepository : Repository<Follow>
{
    public FollowRepository(SNSDbContext context) : base(context) { }
}