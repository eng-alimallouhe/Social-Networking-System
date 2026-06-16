using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Preferences.Repositories;

public class SkillRepository : SoftDeletableRepository<Skill>
{
    public SkillRepository(SNSDbContext context) : base(context) { }
}
