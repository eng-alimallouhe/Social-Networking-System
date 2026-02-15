using SNS.Domain.Security.Entities;
using SNS.Infrastructure.Data;

namespace SNS.Infrastructure.Repositories.Security;

public class RefreshTokenRepository : Repository<RefreshToken>
{
    public RefreshTokenRepository(SNSDbContext context) : base(context) { }
}
