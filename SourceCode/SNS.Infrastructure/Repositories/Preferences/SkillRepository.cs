using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Preferences;

public class SkillRepository : SoftDeletableRepository<Skill>
{
    public SkillRepository(SNSDbContext context) : base(context) { }
}
