namespace SNS.Application.DTOs.Security;

/// <summary>
/// Represents a data transfer object used to
/// provide a lightweight summary of a user session.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client for listing multiple sessions.
/// 
/// It is typically used in the "Active Sessions" or "Login History" screens.
/// </summary>
public class UserSessionSummaryDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the session.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Gets or sets the device name or type.
    /// 
    /// This value is used to help the user identify where they are logged in (e.g., "iPhone 13", "Windows PC").
    /// </summary>
    public string Device { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the browser name and version.
    /// 
    /// This value is used to help the user identify the client used (e.g., "Chrome", "Firefox").
    /// </summary>
    public string Browser { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country of origin based on the IP address.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the IP address of the session.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the session is currently active and valid.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the session was created.
    /// </summary>
    public DateTime LoginAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the session was last used.
    /// </summary>
    public DateTime LastSeenAt { get; set; }

    /// <summary>
    /// Indicates whether this specific session corresponds to the device currently viewing the list.
    /// </summary>
    public bool IsCurrentSession { get; set; }
}