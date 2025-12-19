using SNS.Domain.Security;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class IdentityArchiveRepository : Repository<IdentityArchive>
{
    public IdentityArchiveRepository(SNSDbContext context) : base(context) { }
}
