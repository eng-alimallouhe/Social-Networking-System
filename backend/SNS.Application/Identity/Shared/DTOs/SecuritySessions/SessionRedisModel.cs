namespace SNS.Application.Identity.Shared.DTOs.SecuritySessions;

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
    public Guid SessionId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who owns the session.
    /// </summary>
    public Guid UserId { get; set; }

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
    public Guid DeviceId { get; set; }

    public string FingerprintHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country from which the session originated.
    /// </summary>
    public string Country { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the longitude of the session's origin.
    /// </summary>
    public double Longitude { get; set; }
    
    /// <summary>
    /// Gets or sets the latitude of the session's origin.
    /// </summary>
    public double Latitude { get; set; }


    public string Browser { get; set; } = string.Empty;

    public bool IsDeviceTrusted { get; set; }
}
