using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ProjectContributorRepository : Repository<ProjectContributor>
{
    public ProjectContributorRepository(SNSDbContext context) : base(context) { }
}
