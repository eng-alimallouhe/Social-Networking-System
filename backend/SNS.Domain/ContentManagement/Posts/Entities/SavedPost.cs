using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Posts.Entities;

public class SavedPost : Entity, IHardDeletable
{
    //Primary key
    public Guid Id { get; private set; }

    //Foreign Key: One(Profile) to Many(SavedPosts)
    public Guid ProfileId { get; private set; }

    //Foreign Key: One(Post) to Many(SavedPosts)
    public Guid PostId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public SavedPost()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static SavedPost Create(Guid profileId, Guid postId)
    {
        return new SavedPost
        {
            ProfileId = profileId,
            PostId = postId
        };
    }
}
