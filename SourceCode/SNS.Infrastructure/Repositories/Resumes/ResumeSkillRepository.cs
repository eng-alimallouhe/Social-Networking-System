using SNS.Domain.Resumes.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories;

public class ResumeSkillRepository : Repository<ResumeSkill>
{
    public ResumeSkillRepository(SNSDbContext context) : base(context) { }
}