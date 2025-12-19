using SNS.Domain.Projects.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ProjectMilestoneRepository : Repository<ProjectMilestone>
{
    public ProjectMilestoneRepository(SNSDbContext context) : base(context) { }
}