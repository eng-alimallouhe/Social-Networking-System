using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories;

public class ProjectSkillRepository : Repository<ProjectSkill>
{
    public ProjectSkillRepository(SNSDbContext context) : base(context) { }
}
