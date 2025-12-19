using SNS.Domain.Security;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class PendingUpdateRepository : Repository<PendingUpdate>
{
    public PendingUpdateRepository(SNSDbContext context) : base(context) { }
}
