using SNS.Domain.Jobs.Relations;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.Jobs;

public class SavedJobRepository : Repository<SavedJob>
{
    public SavedJobRepository(SNSDbContext context) : base(context)
    {
    }
}
