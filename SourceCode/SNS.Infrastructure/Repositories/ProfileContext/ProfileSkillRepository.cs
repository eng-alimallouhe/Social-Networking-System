using SNS.Domain.ProfileContext.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.ProfileContext;

public class ProfileSkillRepository : Repository<ProfileSkill>
{
    public ProfileSkillRepository(SNSDbContext context) : base(context) { }
}
