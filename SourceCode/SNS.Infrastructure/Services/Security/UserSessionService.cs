using Microsoft.AspNetCore.Http;
using SNS.Application.Abstractions.Caching;
using SNS.Application.Abstractions.Security;
using SNS.Application.DTOs.Common;
using SNS.Application.DTOs.Security;
using SNS.Common.Results;
using SNS.Common.StatusCodes.Common;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Abstractions.Specifications;
using SNS.Domain.Security.Entities;
using UAParser;

namespace SNS.Application.Services.Security;

public class UserSessionService : IUserSessionService
{
    private readonly IRepository<UserSession> _sessionRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGeoLocationService _geoLocationService;
    private readonly ICacheService _cacheService;
    private readonly Parser _uaParser;

    public UserSessionService(
        IRepository<UserSession> sessionRepo,
        IHttpContextAccessor httpContextAccessor,
        IGeoLocationService geoLocationService,
        ICacheService cacheService)
    {
        _sessionRepo = sessionRepo;
        _httpContextAccessor = httpContextAccessor;
        _geoLocationService = geoLocationService;
        _cacheService = cacheService;
        // Caching the parser instance significantly improves performance in high-throughput scenarios
        _uaParser = Parser.GetDefault();
    }

    public async Task<Result<Guid>> CreateSessionAsync(Guid userId)
    {
        var context = _httpContextAccessor.HttpContext;

        // This check handles edge cases like background jobs invoking the service without an HTTP request
        if (context == null)
        {
            return Result<Guid>.Failure(Resources.ResourceReadError);
        }

        var ipAddress = GetIpAddress(context);
        var uaString = context.Request.Headers["User-Agent"].ToString();

        // Extract client details to populate structured session data
        var clientInfo = _uaParser.Parse(uaString);

        var device = string.IsNullOrWhiteSpace(clientInfo.OS.Family) ? "Unknown" : clientInfo.OS.Family;
        var browser = string.IsNullOrWhiteSpace(clientInfo.UA.Family) ? "Unknown" : clientInfo.UA.Family;

        // Resolving location during creation enables immediate risk analysis and reporting
        var country = await _geoLocationService.GetCountryFromIpAsync(ipAddress);

        var now = DateTime.UtcNow;
        var sessionId = Guid.NewGuid();

        // 1. Prepare SQL Entity (Cold Storage / Audit Trail)
        var sessionEntity = new UserSession
        {
            Id = sessionId,
            UserId = userId,
            LoginAt = now,
            LastSeenAt = now,
            IpAddress = ipAddress,
            Device = device,
            Browser = browser,
            Country = country,
            IsActive = true,
            DurationMinutes = 0
        };

        // Note: Transaction commit depends on the orchestrator (Register/Login Service)
        await _sessionRepo.AddAsync(sessionEntity);

        // 2. Prepare Redis Model (Hot Storage / Fast Access)
        var redisModel = new SessionRedisModel
        {
            SessionId = sessionId.ToString(),
            UserId = userId.ToString(),
            LoginAt = now,
            LastSeenAt = now,
            IpAddress = ipAddress,
            Device = device
        };

        // Set TTL slightly longer than the cleanup worker interval to handle race conditions
        await _cacheService.SetAsync(
            $"session:{sessionId}",
            redisModel,
            TimeSpan.FromMinutes(40));

        // Group active sessions for bulk management (e.g. revoke all)
        await _cacheService.AddToSetAsync(
            $"user:sessions:{userId}",
            sessionId.ToString());

        return Result<Guid>.Success(sessionId, Resources.ResourceFound);
    }

    public async Task<Result<Paged<UserSessionSummaryDto>>> GetSessionsAsync(ISpecification<UserSession> specification)
    {
        // Execute query against persistent storage to support complex filtering criteria
        var (items, count) = await _sessionRepo.GetListAsync(specification);

        // Try to identify the current session from the request context (if available)
        var currentSessionId = TryGetCurrentSessionId();

        // Map Domain Entities to DTOs to hide internal details (like UserId) 
        // and format data for the presentation layer.
        var dtos = items.Select(s => new UserSessionSummaryDto
        {
            SessionId = s.Id,
            Device = s.Device,
            Browser = s.Browser,
            Country = s.Country,
            IpAddress = s.IpAddress,
            IsActive = s.IsActive,
            LoginAt = s.LoginAt,
            LastSeenAt = s.LastSeenAt,
            IsCurrentSession = s.Id == currentSessionId
        }).ToList();

        // Reconstruct pagination metadata
        int pageSize = specification.Take ?? (count == 0 ? 1 : count);
        int skip = specification.Skip ?? 0;
        int currentPage = (skip / pageSize) + 1;

        var pagedResult = new Paged<UserSessionSummaryDto>(dtos, count, pageSize, currentPage);

        return Result<Paged<UserSessionSummaryDto>>.Success(pagedResult, Resources.ResourceFound);
    }

    // --- Helpers ---

    private string GetIpAddress(HttpContext context)
    {
        // Handle reverse proxy headers to get real client IP
        if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            var header = context.Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrWhiteSpace(header))
            {
                return header.Split(',')[0].Trim();
            }
        }
        return context.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
    }

    private Guid? TryGetCurrentSessionId()
    {
        // Attempt to parse the "sid" claim from the current user's token
        var user = _httpContextAccessor.HttpContext?.User;
        var sidClaim = user?.FindFirst("sid")?.Value ?? user?.FindFirst("SessionId")?.Value;

        if (Guid.TryParse(sidClaim, out var guid))
        {
            return guid;
        }
        return null;
    }
}