using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity;

public class UserSecuritySettingsRepository : Repository<UserSecuritySettings>
{
    public UserSecuritySettingsRepository(SNSDbContext context) : base(context)
    {
    }
}
