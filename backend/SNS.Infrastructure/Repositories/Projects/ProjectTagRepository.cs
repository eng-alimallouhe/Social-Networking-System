using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ProjectTagRepository : Repository<ProjectTag>
{
    public ProjectTagRepository(SNSDbContext context) : base(context) { }
}
