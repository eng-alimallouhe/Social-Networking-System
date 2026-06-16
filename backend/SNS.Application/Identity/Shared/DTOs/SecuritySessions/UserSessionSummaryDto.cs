namespace SNS.Application.Identity.Shared.DTOs.SecuritySessions;

/// <summary>
/// Represents a data transfer object used to
/// provide a lightweight summary of a user session.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client for listing multiple sessions.
/// 
/// It is typically used in the "Active Sessions" or "Login History" screens.
/// </summary>
/// <param name="SessionId">Gets the unique identifier of the session.</param>
/// <param name="Device">Gets the device name or type. This value is used to help the user identify where they are logged in (e.g., "iPhone 13", "Windows PC").</param>
/// <param name="Browser">Gets the browser name and version. This value is used to help the user identify the client used (e.g., "Chrome", "Firefox").</param>
/// <param name="Country">Gets the country of origin based on the IP address.</param>
/// <param name="IpAddress">Gets the IP address of the session.</param>
/// <param name="IsActive">Indicates whether the session is currently active and valid.</param>
/// <param name="LoginAt">Gets the date and time when the session was created.</param>
/// <param name="LastSeenAt">Gets the date and time when the session was last used.</param>
/// <param name="IsCurrentSession">Indicates whether this specific session corresponds to the device currently viewing the list.</param>
public record UserSessionSummaryDto(
    Guid SessionId,
    string Device,
    string Browser,
    string Country,
    string IpAddress,
    bool IsActive,
    DateTime LoginAt,
    DateTime LastSeenAt,
    bool IsCurrentSession);
