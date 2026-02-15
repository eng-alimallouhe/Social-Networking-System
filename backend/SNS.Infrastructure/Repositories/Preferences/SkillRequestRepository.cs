using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Preferences;

public class SkillRequestRepository : Repository<SkillRequest>
{
    public SkillRequestRepository(SNSDbContext context) : base(context) { }
}
