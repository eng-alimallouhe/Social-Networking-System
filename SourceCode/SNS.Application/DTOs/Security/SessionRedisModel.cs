namespace SNS.Application.DTOs.Security;

public class SessionRedisModel
{
    public string SessionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime LoginAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
}