using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.SecuritySessions.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Queries.GetUserSessions;

public sealed class GetUserSessionsQueryHandler : IQueryHandler<GetUserSessionsQuery, Paged<SessionSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISessionService _sessionService;
    private readonly ICurrentUserService _currentUserService;

    public GetUserSessionsQueryHandler(
        IApplicationDbContext dbContext,
        ISessionService sessionService,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _sessionService = sessionService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Paged<SessionSummaryDto>>> Handle(GetUserSessionsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return Result<Paged<SessionSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var query = _dbContext.
            UserSessions
            .Where(x => x.UserId == userId);

        if (request.JustActiveSessions)
        {
            query = query.Where(ss => ss.Logout == null);
        }
        
        var response = await query
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ss => new SessionSummaryDto
            (
                ss.Id,
                userId.Value,
                ss.Device.FriendlyName,
                ss.LoginAt,
                ss.LastSeenAt,
                ss.LogoutAt,
                ss.City,
                ss.Country,
                ss.DurationMinutes,
                ss.IsRevoked,
                ss.RevokedReason
            )).ToListAsync(cancellationToken);

        var paged = new Paged<SessionSummaryDto>(
            items: response,
            count: await query.CountAsync(),
            pageSize: request.PageSize, 
            currentPage: request.CurrentPage);

        return Result<Paged<SessionSummaryDto>>.Success(paged, ResourceStatusCode.Found);
    }
}