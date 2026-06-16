using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity;

public class UserPasskeyRepository : Repository<UserPasskey>
{
    public UserPasskeyRepository(SNSDbContext context) : base(context)
    {
    }
}
