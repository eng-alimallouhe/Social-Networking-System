using SNS.Domain.Discussions.Problems.Relations;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Repositories;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Discussions.Problems.Repositories;

public class ProblemTagRepository : Repository<ProblemTag>
{
    public ProblemTagRepository(SNSDbContext context) : base(context) { }
}
