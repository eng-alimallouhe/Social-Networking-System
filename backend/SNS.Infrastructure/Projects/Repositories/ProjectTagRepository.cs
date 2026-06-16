using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories;

public class ProjectTagRepository : Repository<ProjectTag>
{
    public ProjectTagRepository(SNSDbContext context) : base(context) { }
}
