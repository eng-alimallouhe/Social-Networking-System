using SNS.Application.Abstractions.Caching;
using SNS.Application.Abstractions.Security;
using SNS.Application.DTOs.Security;

namespace SNS.Infrastructure.Services.Security;

public class SessionService : ISessionService
{
    private readonly ICacheService _cacheService;

    public SessionService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<bool> ValidateAndUpdateSessionAsync(Guid sessionId)
    {
        var key = $"session:{sessionId}";

        // 1. Try to get from Redis (Fastest path)
        var sessionModel = await _cacheService.GetAsync<SessionRedisModel>(key);

        if (sessionModel == null)
        {
            // Session missing in Redis -> Assume Expired/Revoked.
            // In Hybrid flow, we return false to trigger 401 -> Refresh Token Flow.
            return false;
        }

        // 2. Update LastSeen timestamp (Sliding Expiration logic)
        sessionModel.LastSeenAt = DateTime.UtcNow;

        // 3. Write back to Redis with reset TTL (e.g., 40 mins)
        // This ensures active users never get kicked out as long as they are active.
        await _cacheService.SetAsync(key, sessionModel, TimeSpan.FromMinutes(40));

        return true;
    }
}