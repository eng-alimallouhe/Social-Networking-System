using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Content.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Posts.Bridges;

public class PostView : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid PostId { get; set; }
    public Guid ViewerId { get; set; }

    // Timestamp
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    // Optional Info
    public DeviceType? DeviceType { get; set; }
    public string? IpHash { get; set; }
    public string? Country { get; set; }

    //Soft Delet:
    public bool IsActive { get; set; }

    // Navigation
    public Post Post { get; set; } = null!;
    public Profile Viewer { get; set; } = null!;

    public PostView()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        ViewedAt = DateTime.UtcNow;
        IsActive = true;
    }
}