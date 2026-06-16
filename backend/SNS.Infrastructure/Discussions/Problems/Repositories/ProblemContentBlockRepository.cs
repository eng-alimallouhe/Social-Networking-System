using SNS.Domain.Discussions.Problems.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Repositories;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Discussions.Problems.Repositories;

public class ProblemContentBlockRepository : Repository<ProblemContentBlock>
{
    public ProblemContentBlockRepository(SNSDbContext context) : base(context) { }
}
