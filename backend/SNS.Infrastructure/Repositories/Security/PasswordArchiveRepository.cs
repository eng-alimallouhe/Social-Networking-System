using SNS.Domain.Security.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class PasswordArchiveRepository : Repository<PasswordArchive>
{
    public PasswordArchiveRepository(SNSDbContext context) : base(context) { }
}
