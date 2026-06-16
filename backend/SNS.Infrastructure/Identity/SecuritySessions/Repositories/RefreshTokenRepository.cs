using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity;

public class RefreshTokenRepository : Repository<RefreshToken>
{
    public RefreshTokenRepository(SNSDbContext context) : base(context) { }
}
