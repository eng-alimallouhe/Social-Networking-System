namespace SNS.Application.DTOs.Security;


/// <summary>
/// Represents a lightweight summary of a user session, suitable for lists.
/// </summary>
public class UserSessionSummaryDto
{
    public Guid SessionId { get; set; }
    public string Device { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime LoginAt { get; set; }
    public DateTime LastSeenAt { get; set; }

    public bool IsCurrentSession { get; set; }
}