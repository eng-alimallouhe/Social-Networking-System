using SNS.Domain.Abstractions.Common;
using SNS.Domain.Content.Entities;
using SNS.Domain.Preferences.Entities;

namespace SNS.Domain.Posts.Bridges;

public class PostTag : IHardDeletable
{
    public Guid PostId { get; set; }
    public Guid TagId { get; set; }
    public float? Confidence { get; set; }

    // Navigation
    public Post Post { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}