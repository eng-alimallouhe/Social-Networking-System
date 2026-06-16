using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories;

public class ProjectViewRepository : SoftDeletableRepository<ProjectView>
{
    public ProjectViewRepository(SNSDbContext context) : base(context) { }
}
