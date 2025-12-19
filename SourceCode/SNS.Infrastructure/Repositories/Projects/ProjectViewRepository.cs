using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ProjectViewRepository : Repository<ProjectView>
{
    public ProjectViewRepository(SNSDbContext context) : base(context) { }
}
