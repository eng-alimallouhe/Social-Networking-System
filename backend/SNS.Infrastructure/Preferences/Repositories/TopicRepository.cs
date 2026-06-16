using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Preferences.Repositories;

public class TopicRepository : Repository<Topic>
{
    public TopicRepository(SNSDbContext context) : base(context) { }
}
