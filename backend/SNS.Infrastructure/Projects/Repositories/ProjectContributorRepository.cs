using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories;

public class ProjectContributorRepository : Repository<ProjectContributor>
{
    public ProjectContributorRepository(SNSDbContext context) : base(context) { }
}
