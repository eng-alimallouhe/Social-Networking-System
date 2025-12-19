using SNS.Domain.Security;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class ManualRecoveryRequestRepository : Repository<ManualRecoveryRequest>
{
    public ManualRecoveryRequestRepository(SNSDbContext context) : base(context) { }
}
