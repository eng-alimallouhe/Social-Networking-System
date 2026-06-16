using Microsoft.EntityFrameworkCore;
using SNS.Domain.Discussions.Solutions.Relations;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.QA;

public class SavedSolutionRepository : Repository<SavedSolution>
{
    public SavedSolutionRepository(SNSDbContext context) : base(context)
    {
    }
}
