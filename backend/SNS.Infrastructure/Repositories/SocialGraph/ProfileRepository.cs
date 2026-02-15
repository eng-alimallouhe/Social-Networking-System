using SNS.Domain.SocialGraph;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.SocialGraph;

public class ProfileRepository : SoftDeletableRepository<Profile>
{
    public ProfileRepository(SNSDbContext context) : base(context) { }
}
