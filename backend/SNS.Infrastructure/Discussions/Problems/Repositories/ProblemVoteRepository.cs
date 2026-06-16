using SNS.Domain.Discussions.Problems.Relations;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Repositories;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Discussions.Problems.Repositories;

public class ProblemVoteRepository : Repository<ProblemVote>
{
    public ProblemVoteRepository(SNSDbContext context) : base(context) { }
}
