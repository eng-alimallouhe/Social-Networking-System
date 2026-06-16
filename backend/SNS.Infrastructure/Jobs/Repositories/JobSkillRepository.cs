using SNS.Domain.Jobs.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.Jobs;

public class JobSkillRepository : Repository<JobSkill>
{
    public JobSkillRepository(SNSDbContext context) : base(context) { }
}
