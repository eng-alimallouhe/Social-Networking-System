using SNS.Domain.ProfileContext.Bridges;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.ProfileContext;

public class ProfileTopicRepository : Repository<ProfileTopic>
{
    public ProfileTopicRepository(SNSDbContext context) : base(context) { }
}