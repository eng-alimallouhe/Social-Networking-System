using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Preferences;

public class TopicRepository : Repository<Topic>
{
    public TopicRepository(SNSDbContext context) : base(context) { }
}
