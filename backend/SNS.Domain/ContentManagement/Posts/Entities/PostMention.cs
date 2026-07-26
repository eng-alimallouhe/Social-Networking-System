using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;

namespace SNS.Domain.ContentManagement.Posts.Entities;

public class PostMention : Entity, IHardDeletable
{
    public Guid ProfileId { get; set; }
    public Guid PostId { get; set; }

    public Profile Profile { get; set; } = null!;
    public Post Post { get; set; } = null!;
}
