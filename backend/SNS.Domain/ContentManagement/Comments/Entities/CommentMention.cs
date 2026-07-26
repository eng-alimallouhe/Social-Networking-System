using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;

namespace SNS.Domain.ContentManagement.Comments.Entities;

public class CommentMention : Entity, IHardDeletable
{
    public Guid ProfileId { get; set; }
    public Guid CommentId { get; set; }
    public Comment Comment { get; set; } = null!;
    public Profile Profile { get; set; } = null!;
}