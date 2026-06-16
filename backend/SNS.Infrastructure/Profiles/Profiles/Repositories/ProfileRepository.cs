using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Profiles.Profiles.Repositories;

public class ProfileRepository : SoftDeletableRepository<Profile>
{
    public ProfileRepository(SNSDbContext context) : base(context) { }
}
