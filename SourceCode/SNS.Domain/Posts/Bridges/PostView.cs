using SNS.Domain.Common.Enums;
using SNS.Domain.Content.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Posts.Bridges;

public class PostView
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid PostId { get; set; }
    public Guid ViewerProfileId { get; set; }

    // Timestamp
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    // Optional Info
    public DeviceType? DeviceType { get; set; }
    public string? IpHash { get; set; }
    public string? Country { get; set; }

    // Navigation
    public Post Post { get; set; } = null!;
    public Profile ViewerProfile { get; set; } = null!;
}