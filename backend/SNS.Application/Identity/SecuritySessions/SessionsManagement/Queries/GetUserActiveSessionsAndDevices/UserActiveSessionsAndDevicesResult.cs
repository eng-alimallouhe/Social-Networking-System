namespace SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserActiveSessionsAndDevices;

#region 🎁 Data Transfer Objects (DTOs)

public sealed record ActiveSessionDto(
    Guid SessionId,
    string DeviceName,
    string Browser,
    string OperatingSystem,
    string IpAddress,
    string? Location,
    DateTime CreatedAt,
    bool IsCurrentSession);



public sealed record RegisteredDeviceDto(
    Guid DeviceId,
    string DeviceName,
    string OperatingSystem,
    DateTime FirstSeenAt,
    DateTime LastSeenAt);


public sealed record UserActiveSessionsAndDevicesResult(
    IReadOnlyCollection<ActiveSessionDto> ActiveSessions,
    IReadOnlyCollection<RegisteredDeviceDto> RegisteredDevices);

#endregion
