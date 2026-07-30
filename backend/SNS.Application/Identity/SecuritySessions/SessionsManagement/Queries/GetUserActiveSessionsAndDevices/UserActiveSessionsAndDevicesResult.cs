namespace SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserActiveSessionsAndDevices;

#region 🎁 Data Transfer Objects (DTOs)

/// <summary>
/// Represents a data transfer object containing active security session details.
/// </summary>
/// <param name="SessionId">The unique identifier of the security session.</param>
/// <param name="DeviceName">The name of the client device.</param>
/// <param name="Browser">The web browser used during the session.</param>
/// <param name="OperatingSystem">The operating system of the client device.</param>
/// <param name="IpAddress">The IP address associated with the session.</param>
/// <param name="Location">The geographical location derived from the IP address.</param>
/// <param name="CreatedAt">The date and time when the session was created.</param>
/// <param name="IsCurrentSession">Indicates whether this session is the active request session.</param>
public sealed record ActiveSessionDto(
    Guid SessionId,
    string DeviceName,
    string Browser,
    string OperatingSystem,
    string IpAddress,
    string? Location,
    DateTime CreatedAt,
    bool IsCurrentSession);

/// <summary>
/// Represents a data transfer object containing registered device information.
/// </summary>
/// <param name="DeviceId">The unique identifier of the registered device.</param>
/// <param name="DeviceName">The user-friendly name of the device.</param>
/// <param name="OperatingSystem">The operating system running on the device.</param>
/// <param name="FirstSeenAt">The timestamp when the device was first registered or logged in.</param>
/// <param name="LastSeenAt">The timestamp when the device was last active.</param>
public sealed record RegisteredDeviceDto(
    Guid DeviceId,
    string DeviceName,
    string OperatingSystem,
    DateTime FirstSeenAt,
    DateTime LastSeenAt);

/// <summary>
/// Represents the query response containing all active security sessions and registered devices for a user.
/// </summary>
/// <param name="ActiveSessions">The collection of active security sessions.</param>
/// <param name="RegisteredDevices">The collection of registered user devices.</param>
public sealed record UserActiveSessionsAndDevicesResult(
    IReadOnlyCollection<ActiveSessionDto> ActiveSessions,
    IReadOnlyCollection<RegisteredDeviceDto> RegisteredDevices);

#endregion

