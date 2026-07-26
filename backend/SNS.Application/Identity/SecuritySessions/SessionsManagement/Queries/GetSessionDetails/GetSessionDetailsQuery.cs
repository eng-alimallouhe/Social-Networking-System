using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Identity.Users.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetSessionDetails;

public sealed record SessionDetaildDto(
    Guid SessionId,
    Guid UserId,
    DateTime LoginAt,
    DateTime LastSeenAt,
    DateTime? LogoutAt,
    string IpAddress,
    string City,
    string Country,
    int DurationMinutes,
    DateTime? RevokedAt,
    bool IsRevoked,
    string? RevokedReason,
    string DeviceName,
    string Browser,
    string OperatingSystem,
    string? DeviceVendor,
    bool IsDeviceTrusted,
    DateTime DeviceFirstSeenAt,
    bool IsViewrOwner,
    bool IsViwerCurrentSession);

public sealed record GetSessionDetailsQuery(Guid SessionId): IQuery<SessionDetaildDto?>;

public sealed class GetSessionDetailsQueryHandler : IQueryHandler<GetSessionDetailsQuery, SessionDetaildDto?>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetSessionDetailsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<SessionDetaildDto?>> Handle(GetSessionDetailsQuery request, CancellationToken cancellationToken)
    {
        var viwerId = _currentUserService.UserId;
        var viwerSessionId = _currentUserService.SessionId;
        var requestSessionId = request.SessionId;
        var isViwerAdmin = _currentUserService.RoleType != null && _currentUserService.RoleType.Contains(RoleType.Admin.ToString(), StringComparison.OrdinalIgnoreCase);

        var session = await _dbContext
            .UserSessions
            .Where(ss => ss.Id == requestSessionId && (ss.UserId == viwerId || isViwerAdmin))
            .Select(ss => new SessionDetaildDto(
                ss.Id,
                ss.UserId,
                ss.LoginAt,
                ss.LastSeenAt,
                ss.LogoutAt,
                ss.IpAddress,
                ss.City,
                ss.Country,
                ss.DurationMinutes,
                ss.RevokedAt,
                ss.IsRevoked,
                ss.RevokedReason,
                ss.Device.FriendlyName,
                ss.Device.Browser,
                ss.Device.OperatingSystem,
                ss.Device.DeviceVendor,
                ss.Device.IsTrusted,
                ss.Device.FirstSeenAt,
                ss.UserId == viwerId,
                ss.Id == viwerSessionId
                ))
            .FirstOrDefaultAsync(cancellationToken);

        if (session == null)
        {
            return Result<SessionDetaildDto?>.Failure(ResourceStatusCode.NotFound);
        }

        return Result<SessionDetaildDto?>.Success(session, ResourceStatusCode.Found);
    }
}
