using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Profiles.SocialGraph.Repositories;

public class BlockRepository : Repository<Block>
{
    public BlockRepository(SNSDbContext context) : base(context) { }
}
