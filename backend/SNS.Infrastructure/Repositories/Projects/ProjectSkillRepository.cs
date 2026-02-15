using SNS.Domain.Projects.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ProjectSkillRepository : Repository<ProjectSkill>
{
    public ProjectSkillRepository(SNSDbContext context) : base(context) { }
}
