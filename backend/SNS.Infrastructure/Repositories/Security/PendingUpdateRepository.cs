using SNS.Domain.Security.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class PendingUpdateRepository : Repository<PendingUpdate>
{
    public PendingUpdateRepository(SNSDbContext context) : base(context) { }
}
