using SNS.Domain.Profiles.Profiles.Relations;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.ProfileContext;

public class ProfileSkillRepository : Repository<ProfileSkill>
{
    public ProfileSkillRepository(SNSDbContext context) : base(context) { }
}
