namespace SNS.Application.Identity.Shared.DTOs.SecuritySessions;

/// <summary>
/// Represents a data transfer object used to
/// provide comprehensive details about a specific user session.
/// </summary>
/// <param name="SessionId">Gets the unique identifier of the session.</param>
/// <param name="Device">Gets the device name or type.</param>
/// <param name="Browser">Gets the browser name and version.</param>
/// <param name="Country">Gets the country of origin based on the IP address.</param>
/// <param name="IpAddress">Gets the IP address of the session.</param>
/// <param name="IsActive">Indicates whether the session is currently active and valid.</param>
/// <param name="LoginAt">Gets the date and time when the session was created.</param>
/// <param name="LastSeenAt">Gets the date and time when the session was last used.</param>
/// <param name="IsCurrentSession">Indicates whether this specific session corresponds to the device currently viewing the list.</param>
/// <param name="DurationMinutes">Gets the duration of the session in minutes. Optional.</param>
/// <param name="LogoutAt">Gets the date and time when the session ended. Optional.</param>
public sealed record UserSessionDetailsDto(
    Guid SessionId,
    string Device,
    string Browser,
    string Country,
    string IpAddress,
    bool IsActive,
    DateTime LoginAt,
    DateTime LastSeenAt,
    bool IsCurrentSession,
    int? DurationMinutes,
    DateTime? LogoutAt) 
    : UserSessionSummaryDto(SessionId, Device, Browser, Country, IpAddress, IsActive, LoginAt, LastSeenAt, IsCurrentSession);
