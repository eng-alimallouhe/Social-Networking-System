using SNS.Domain.Security;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class UserRepository : SoftDeletableRepository<User>
{
    public UserRepository(SNSDbContext context) : base(context) { }
}
