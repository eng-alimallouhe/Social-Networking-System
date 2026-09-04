using Microsoft.EntityFrameworkCore;
using SNS.Domain.Identity.SecuritySessions.Abstractions;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity.SecuritySessions.Repositories;

public class SecuritySessionRepository : Repository<SecuritySession>, ISessionRepository
{
    private readonly SNSDbContext _dbContext;
    public SecuritySessionRepository(SNSDbContext context) : base(context) 
    { 
        _dbContext = context;
    }

    public async Task UpdateSessionLastSeenAsync(Guid sessionId, DateTime lastSeen, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserSessions
            .Where(s => s.Id == sessionId)
            .ExecuteUpdateAsync(a => a.SetProperty(s => s.LastSeenAt, lastSeen), cancellationToken);
    }
}
