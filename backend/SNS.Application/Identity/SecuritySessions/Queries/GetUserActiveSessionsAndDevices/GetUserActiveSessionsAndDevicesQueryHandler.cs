using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Queries.GetUserActiveSessionsAndDevices;

public sealed class GetUserActiveSessionsAndDevicesQueryHandler
    : IQueryHandler<GetUserActiveSessionsAndDevicesQuery, UserActiveSessionsAndDevicesResult>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetUserActiveSessionsAndDevicesQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserActiveSessionsAndDevicesResult>> Handle(
        GetUserActiveSessionsAndDevicesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null || userId == Guid.Empty)
        {
            return Result<UserActiveSessionsAndDevicesResult>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var dbData = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                ActiveSessions = u.Sessions
                    .Where(s => !s.IsRevoked)
                    .OrderByDescending(s => s.LoginAt)
                    .Select(s => new
                    {
                        s.Id,
                        s.Device.FriendlyName,
                        s.Device.Browser,
                        s.Device.OperatingSystem,
                        s.IpAddress,
                        Location = s.City != null ? $"{s.City}, {s.Country}" : s.Country,
                        s.LoginAt
                    }),

                RegisteredDevices = u.Devices
                    .OrderByDescending(d => d.FirstSeenAt) 
                    .Take(5)
                    .Select(d => new
                    {
                        d.Id,
                        d.FriendlyName,
                        d.OperatingSystem,
                        FirstSeenAt = d.FirstSeenAt,
                        LastSeenAt = d.LastSeenAt,
                    })
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dbData == null)
        {
            return Result<UserActiveSessionsAndDevicesResult>.Failure(UserStatusCodes.NotFound);
        }

        var currentSessionId = _currentUserService.SessionId;

        var activeSessions = dbData.ActiveSessions.Select(s => new ActiveSessionDto(
            SessionId: s.Id,
            DeviceName: s.FriendlyName,
            Browser: s.Browser,
            OperatingSystem: s.OperatingSystem,
            IpAddress: s.IpAddress,
            Location: s.Location,
            CreatedAt: s.LoginAt,
            IsCurrentSession: currentSessionId.HasValue && s.Id == currentSessionId.Value
        )).ToList();

        var registeredDevices = dbData.RegisteredDevices.Select(d => new RegisteredDeviceDto(
            DeviceId: d.Id,
            DeviceName: d.FriendlyName,
            OperatingSystem: d.OperatingSystem,
            FirstSeenAt: d.FirstSeenAt,
            LastSeenAt: d.LastSeenAt
        )).ToList();

        var result = new UserActiveSessionsAndDevicesResult(
            ActiveSessions: activeSessions.AsReadOnly(),
            RegisteredDevices: registeredDevices.AsReadOnly()
        );

        return Result<UserActiveSessionsAndDevicesResult>.Success(result, OperationStatusCode.Success);
    }
}