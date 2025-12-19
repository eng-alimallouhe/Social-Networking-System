using SNS.Domain.QA.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.QA;

public class ProblemVoteRepository : Repository<ProblemVote>
{
    public ProblemVoteRepository(SNSDbContext context) : base(context) { }
}
