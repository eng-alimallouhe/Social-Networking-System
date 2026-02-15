using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Preferences;

public class InterestRepository : SoftDeletableRepository<Interest>
{
    public InterestRepository(SNSDbContext context) : base(context) { }
}
