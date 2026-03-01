using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Content.Enums;

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

    public PostReaction()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
