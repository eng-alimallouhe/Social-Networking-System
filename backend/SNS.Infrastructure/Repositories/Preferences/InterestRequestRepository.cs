using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Preferences;

public class InterestRequestRepository : Repository<InterestRequest>
{
    public InterestRequestRepository(SNSDbContext context) : base(context)
    {
    }
}
