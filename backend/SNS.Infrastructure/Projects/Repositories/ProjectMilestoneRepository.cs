using SNS.Domain.Projects.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories;

public class ProjectMilestoneRepository : Repository<ProjectMilestone>
{
    public ProjectMilestoneRepository(SNSDbContext context) : base(context) { }
}
