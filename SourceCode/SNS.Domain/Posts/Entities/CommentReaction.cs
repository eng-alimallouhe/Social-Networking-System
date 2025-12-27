using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Content.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Content.Entities;

public class CommentReaction : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid CommentId { get; set; }
    public Guid ReactingProfileId { get; set; }

    // General
    public ReactionType Type { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Comment Comment { get; set; } = null!;
    public Profile Profile { get; set; } = null!;

    public CommentReaction()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }
}