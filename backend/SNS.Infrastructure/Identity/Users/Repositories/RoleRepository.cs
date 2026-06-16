using SNS.Domain.Identity.Users.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity.Users.Repositories;

public class RoleRepository : SoftDeletableRepository<Role>
{
    public RoleRepository(SNSDbContext context) : base(context) { }
}
