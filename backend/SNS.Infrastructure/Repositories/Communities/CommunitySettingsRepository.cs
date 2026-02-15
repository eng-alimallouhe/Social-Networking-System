using SNS.Domain.Communities.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Communities
{

    public class CommunitySettingsRepository : Repository<CommunitySettings>
    {
        public CommunitySettingsRepository(SNSDbContext context) : base(context) { }
    }
}
