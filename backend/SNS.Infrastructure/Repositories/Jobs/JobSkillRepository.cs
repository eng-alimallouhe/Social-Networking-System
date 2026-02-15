using SNS.Domain.Jobs.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Jobs;

public class JobSkillRepository : Repository<JobSkill>
{
    public JobSkillRepository(SNSDbContext context) : base(context) { }
}