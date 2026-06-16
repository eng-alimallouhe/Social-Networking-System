using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Profiles.Profiles.Repositories;

public class ProfileViewRepository : SoftDeletableRepository<ProfileView>
{
    public ProfileViewRepository(SNSDbContext context) : base(context)
    {
    }
}
