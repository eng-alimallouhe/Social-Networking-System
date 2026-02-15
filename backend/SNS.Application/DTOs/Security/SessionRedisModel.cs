namespace SNS.Application.DTOs.Security;

/// <summary>
/// Represents a data transfer object used to
/// store session data in a distributed Redis cache.
/// 
/// This DTO is designed to transfer data between
/// the application and the Redis store for high-performance access
/// without accessing the primary database.
/// 
/// It is typically used in authentication middleware and session tracking.
/// </summary>
public class SessionRedisModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the session.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the user who owns the session.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the session started.
    /// </summary>
    public DateTime LoginAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user was last active.
    /// 
    /// This value is used to calculate session timeouts.
    /// </summary>
    public DateTime LastSeenAt { get; set; }

    /// <summary>
    /// Gets or sets the IP address from which the session originated.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the device information (e.g., User-Agent string).
    /// </summary>
    public string Device { get; set; } = string.Empty;
}