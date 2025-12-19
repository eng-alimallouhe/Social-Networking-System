using SNS.Domain.QA.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.QA;

public class SolutionVoteRepository : Repository<SolutionVote>
{
    public SolutionVoteRepository(SNSDbContext context) : base(context) { }
}
