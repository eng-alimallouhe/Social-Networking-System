using SNS.Domain.SocialGraph;
using SNS.Domain.Content.Enums;
using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Content.Entities;

public class PostReaction : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid PostId { get; set; }
    public Guid ReactingProfileId { get; set; }

    // General
    public ReactionType ReactionType { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Post Post { get; set; } = null!;
    public Profile Profile { get; set; } = null!;
}
