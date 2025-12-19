using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Preferences;

public class TopicInterestRepository : Repository<TopicInterest>
{
    public TopicInterestRepository(SNSDbContext context) : base(context) { }
}