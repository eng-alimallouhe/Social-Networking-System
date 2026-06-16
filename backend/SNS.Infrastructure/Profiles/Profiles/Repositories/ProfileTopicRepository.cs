using SNS.Domain.Profiles.Profiles.Relations;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Repositories.ProfileContext;

public class ProfileTopicRepository : Repository<ProfileTopic>
{
    public ProfileTopicRepository(SNSDbContext context) : base(context) { }
}
