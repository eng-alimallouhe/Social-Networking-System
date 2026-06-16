using SNS.Domain.Discussions.Problems.Relations;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Repositories;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Discussions.Problems.Repositories;

public class SavedProblemRepository : Repository<SavedProblem>
{
    public SavedProblemRepository(SNSDbContext context) : base(context)
    {
    }
}
