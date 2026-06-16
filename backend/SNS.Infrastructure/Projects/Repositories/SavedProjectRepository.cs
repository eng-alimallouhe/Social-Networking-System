using Microsoft.EntityFrameworkCore;
using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.Projects;

public class SavedProjectRepository : Repository<SavedProject>
{
    public SavedProjectRepository(SNSDbContext context) : base(context)
    {
    }
}
