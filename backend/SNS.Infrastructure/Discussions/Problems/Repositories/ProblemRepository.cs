using SNS.Domain.Discussions.Problems.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Repositories;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Discussions.Problems.Repositories;

public class ProblemRepository : SoftDeletableRepository<Problem>
{
    public ProblemRepository(SNSDbContext context) : base(context) { }
}
