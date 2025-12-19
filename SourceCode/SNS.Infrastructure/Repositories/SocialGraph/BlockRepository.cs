using SNS.Domain.SocialGraph.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.SocialGraph;

public class BlockRepository : Repository<Block>
{
    public BlockRepository(SNSDbContext context) : base(context) { }
}
