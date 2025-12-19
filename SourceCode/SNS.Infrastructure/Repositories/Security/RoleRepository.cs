using SNS.Domain.Security;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class RoleRepository : SoftDeletableRepository<Role>
{
    public RoleRepository(SNSDbContext context) : base(context) { }
}
