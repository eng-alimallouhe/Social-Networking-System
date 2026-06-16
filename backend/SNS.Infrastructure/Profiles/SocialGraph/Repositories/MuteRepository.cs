using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Profiles.SocialGraph.Repositories;

public class MuteRepository : Repository<Mute>
{
    public MuteRepository(SNSDbContext context) : base(context) { }
}
