using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ProjectViewRepository : SoftDeletableRepository<ProjectView>
{
    public ProjectViewRepository(SNSDbContext context) : base(context) { }
}
