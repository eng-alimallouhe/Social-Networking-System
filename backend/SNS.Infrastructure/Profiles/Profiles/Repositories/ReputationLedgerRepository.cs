using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Profiles.Profiles.Repositories;

public class ReputationLedgerRepository : Repository<ReputationLedger>
{
    public ReputationLedgerRepository(SNSDbContext context) : base(context)
    {
    }
}
