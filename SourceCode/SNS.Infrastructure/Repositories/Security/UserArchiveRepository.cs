using SNS.Domain.Security;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class UserArchiveRepository : Repository<UserArchive>
{
    public UserArchiveRepository(SNSDbContext context) : base(context) { }
}
