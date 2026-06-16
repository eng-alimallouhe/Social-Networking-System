using SNS.Domain.Identity.Users.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity.Users.Repositories;

public class UserRepository : Repository<User>
{
    public UserRepository(SNSDbContext context) : base(context) { }
}
