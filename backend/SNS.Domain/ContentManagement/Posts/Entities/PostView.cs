using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Posts.Entities;

public class PostView : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Post) to Many(PostView)
    public Guid PostId { get; private set; }

    // Foreign Key: One(Profile) to Many(PostView)
    public Guid ViewerId { get; private set; }

    // Timestamp
    public DateTime ViewedAt { get; private set; } = DateTime.UtcNow;

    // Optional Info
    public DeviceType? DeviceType { get; private set; }
    public string? IpHash { get; private set; }
    public string? Country { get; private set; }
    public string? City { get; private set; }

    //Soft Delet:
    public bool IsActive { get; private set; }

    public PostView()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        ViewedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void SoftDelete()
    {
        this.IsActive = false;
    }

    public static PostView Create(
        Guid postId, 
        Guid viewerId,
        string? ipHash = null,
        string? country = null,
        string? city = null,
        DeviceType? deviceType = null)
    {
        return new PostView()
        {
            PostId = postId,
            ViewerId = viewerId,
            IpHash = ipHash,
            Country = country,
            City = city, 
            DeviceType = deviceType
        };
    }
}
