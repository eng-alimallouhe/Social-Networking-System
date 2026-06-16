using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Profiles.SocialGraph.Repositories;

public class FollowRepository : Repository<Follow>
{
    public FollowRepository(SNSDbContext context) : base(context) { }
}
