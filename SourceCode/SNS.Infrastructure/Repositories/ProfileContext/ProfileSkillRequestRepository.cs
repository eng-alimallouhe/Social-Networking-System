using SNS.Domain.ProfileContext.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.ProfileContext;

public class ProfileSkillRequestRepository : Repository<ProfileSkillRequest>
{
    public ProfileSkillRequestRepository(SNSDbContext context) : base(context) { }
}
