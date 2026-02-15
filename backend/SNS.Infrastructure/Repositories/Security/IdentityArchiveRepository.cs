using SNS.Domain.Security.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class IdentityArchiveRepository : Repository<IdentityArchive>
{
    public IdentityArchiveRepository(SNSDbContext context) : base(context) { }
}
