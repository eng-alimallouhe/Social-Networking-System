using SNS.Domain.Identity.Users.Relations;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity.Users.Repositories;

public class RolePermissionRepository : Repository<RolePermission>
{
    public RolePermissionRepository(SNSDbContext context) : base(context) { }
}
