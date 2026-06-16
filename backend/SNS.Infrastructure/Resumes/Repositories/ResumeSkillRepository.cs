using SNS.Domain.Resumes.Bridges;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Resumes.Repositories;

public class ResumeSkillRepository : Repository<ResumeSkill>
{
    public ResumeSkillRepository(SNSDbContext context) : base(context) { }
}
