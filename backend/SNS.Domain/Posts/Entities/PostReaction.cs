using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Content.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Content.Entities;

public class PostReaction : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid PostId { get; set; }
    public Guid ReactorId { get; set; }

    // General
    public ReactionType Type { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Post Post { get; set; } = null!;
    public Profile Reactor { get; set; } = null!;

    public PostReaction()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }
}