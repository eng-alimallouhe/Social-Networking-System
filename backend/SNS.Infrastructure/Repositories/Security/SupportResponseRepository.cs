using SNS.Domain.Security.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class SupportResponseRepository : Repository<SupportResponse>
{
    public SupportResponseRepository(SNSDbContext context) : base(context) { }
}
