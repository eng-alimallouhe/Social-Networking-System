using SNS.Application.Abstractions.Caching;
using SNS.Application.Abstractions.Common;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Identity.SecuritySessions.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Shared.Services;

public class SessionService : ISessionService
{
    private readonly IRepository<SecuritySession> _sessionRepo;
    private readonly ICacheService _cacheService;
    private readonly IIdentityCacheKeyFactory _identityCacheKeyFactory;
    private readonly TimeSpan _sessionCacheDuration = TimeSpan.FromMinutes(40);
    private readonly IGeneratorService _generatorService;

    public SessionService(
        IRepository<SecuritySession> sessionRepo,
        ICacheService cacheService,
        IIdentityCacheKeyFactory identityCacheKeyFactory, 
        IGeneratorService generatorService)
    {
        _sessionRepo = sessionRepo;
        _cacheService = cacheService;
        _identityCacheKeyFactory = identityCacheKeyFactory;
        _generatorService = generatorService;
    }

    public async Task<bool> ValidateAndUpdateSessionAsync(
        Guid sessionId, 
        Guid userId,
        CancellationToken cancellationToken)
    {
        var key = _identityCacheKeyFactory.GetSessionKey(sessionId);

        var sessionModel = await _cacheService.GetAsync<SessionRedisModel>(key, cancellationToken);

        if (sessionModel != null)
        {
            sessionModel.LastSeenAt = DateTime.UtcNow;
            await _cacheService.SetAsync(key, sessionModel, _sessionCacheDuration, cancellationToken);
            return true;
        }

        var sessionEntity = await _sessionRepo.GetByIdAsync(sessionId, cancellationToken);

        if (sessionEntity == null || !sessionEntity.IsActive || sessionEntity.UserId != userId || sessionEntity.LogoutAt != null)
        {
            return false;
        }

        sessionModel = new SessionRedisModel
        {
            SessionId = sessionEntity.Id,
            UserId = sessionEntity.UserId,
            LoginAt = sessionEntity.LoginAt,
            LastSeenAt = DateTime.UtcNow,
            IpAddress = sessionEntity.IpAddress,
            DeviceId = sessionEntity.DeviceId,
        };

        await _cacheService.SetAsync(key, sessionModel, _sessionCacheDuration, cancellationToken);

        return true;
    }


    public async Task<Result<SessionCreateResult>> CreateSessionAsync(
        CreateSessionDto dto, 
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var oldSessionSpec =  new CurrentSecuritySessionByDeviceIdAndUserIdSpecification(dto.UserId, dto.DeviceId);

        var oldSession = await _sessionRepo.GetSingleAsync(oldSessionSpec, cancellationToken);

        if (oldSession != null)
        {
            oldSession.Logout(at: now);
            oldSession.SetDurationMinutes(durationMinutes: (int)(now - oldSession.LoginAt).TotalMinutes);
            
            await _cacheService.RemoveAsync(_identityCacheKeyFactory.GetSessionKey(oldSession.Id), cancellationToken);
        }

        var token = _generatorService.GenerateSecureString();

        var sessionEntity = SecuritySession.Create(
            userId: dto.UserId,
            ipAddress: dto.IpAddress,
            deviceId: dto.DeviceId,
            city: dto.City,
            country: dto.Country,
            durationMinutes: 0,
            refreshToken: token,
            tokenExpiresAt: now.AddDays(7)
        );

        _sessionRepo.Add(sessionEntity);

        var redisModel = new SessionRedisModel
        {
            SessionId = sessionEntity.Id,
            UserId = dto.UserId,
            DeviceId = dto.DeviceId,
            LoginAt = now,
            LastSeenAt = now,
            IpAddress = dto.IpAddress,
            Longitude = dto.Longitude,
            Country = dto.Country,
            Latitude = dto.Latitude,
            Browser = dto.Browser,
        };

        var sessionKey = _identityCacheKeyFactory.GetSessionKey(sessionEntity.Id);

        var userSessionsKey = _identityCacheKeyFactory.GetUserSessionsKey(dto.UserId);

        await _cacheService.SetAsync(
            sessionKey,
            redisModel,
            _sessionCacheDuration,
            cancellationToken);

        await _cacheService.AddToSetAsync(
            userSessionsKey,
            sessionEntity.Id.ToString(),
            cancellationToken);

        return Result<SessionCreateResult>.Success(new SessionCreateResult(sessionEntity.Id, sessionEntity.RefreshToken), ResourceStatusCode.Found);
    }


    public async Task<Result> ClearSessionsByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var spec = new CurrentSecuritySessionsSpecification(userId);

        var currentSessionsForUser = await _sessionRepo.GetListAsync(spec, cancellationToken);

        foreach (var session in currentSessionsForUser)
        {
            session.Logout(at: DateTime.UtcNow);

            var sessionKey = _identityCacheKeyFactory.GetSessionKey(session.Id);
            await _cacheService.RemoveAsync(sessionKey, cancellationToken);
        }

        var userSessionsKey = _identityCacheKeyFactory.GetUserSessionsKey(userId);
        await _cacheService.RemoveAsync(userSessionsKey, cancellationToken);
        return Result.Success(OperationStatusCode.Success);
    }


    public async Task<Result> KillSwitchSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var spec = new SecuritySessionWithRefreshTokens(sessionId);

        var session = await _sessionRepo.GetSingleAsync(spec, cancellationToken);
        
        if (session == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        session.Logout(at: DateTime.UtcNow);

        await _cacheService.RemoveAsync(_identityCacheKeyFactory.GetSessionKey(sessionId), cancellationToken);

        await _cacheService.RemoveFromSetAsync(
            _identityCacheKeyFactory.GetUserSessionsKey(session.UserId), 
            sessionId.ToString(), 
            cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }


    public async Task<List<SessionRedisModel>> GetActiveSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userSessionsKey = _identityCacheKeyFactory.GetUserSessionsKey(userId);
        
        var sessionIds = await _cacheService.GetSetMembersAsync(userSessionsKey, cancellationToken);
        
        var sessions = new List<SessionRedisModel>();
        
        foreach (var sessionId in sessionIds)
        {
            var sessionKey = _identityCacheKeyFactory.GetSessionKey(Guid.Parse(sessionId));
            var sessionModel = await _cacheService.GetAsync<SessionRedisModel>(sessionKey, cancellationToken);
            if (sessionModel != null)
            {
                sessions.Add(sessionModel);
            }
        }
        return sessions;
    }

    public async Task<Result<string>> RotateRefreshTokenAsync(
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepo.GetByIdAsync(
            sessionId,
            cancellationToken);

        if (session == null)
        {
            return Result<string>.Failure(ResourceStatusCode.NotFound);
        }

        if (!session.IsActive ||
            session.UserId != userId ||
            session.LogoutAt != null ||
            session.IsRevoked)
        {
            return Result<string>.Failure(SecurityStatusCodes.UnAuthorized);
        }

        var refreshToken = _generatorService.GenerateSecureString();

        var expiresAt = DateTime.UtcNow.AddDays(7);

        session.UpdateRefreshToken(
            newRefreshToken: refreshToken,
            newExpiresAt: expiresAt);

        session.UpdateLastSeen();

        return Result<string>.Success(
            refreshToken,
            OperationStatusCode.Success);
    }
}
