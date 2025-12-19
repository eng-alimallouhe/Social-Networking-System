using SNS.Domain.Security;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class SupportResponseRepository : Repository<SupportResponse>
{
    public SupportResponseRepository(SNSDbContext context) : base(context) { }
}
