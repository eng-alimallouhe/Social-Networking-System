using SNS.Domain.Discussions.Solutions.Relations;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.QA;

public class SolutionVoteRepository : Repository<SolutionVote>
{
    public SolutionVoteRepository(SNSDbContext context) : base(context) { }
}
