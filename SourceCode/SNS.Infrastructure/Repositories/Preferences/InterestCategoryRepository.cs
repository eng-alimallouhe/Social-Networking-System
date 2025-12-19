using SNS.Domain.Preferences.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Preferences;

public class InterestCategoryRepository : SoftDeletableRepository<InterestCategory>
{
    public InterestCategoryRepository(SNSDbContext context) : base(context) { }
}
