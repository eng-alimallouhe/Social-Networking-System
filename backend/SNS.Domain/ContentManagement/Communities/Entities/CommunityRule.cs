using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Communities.Entities;

public class CommunityRule : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Community) ? Many(Rules)
    public Guid CommunityId { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Order { get; private set; }

    // Navigation Property
    public Community Community { get; set; } = null!;

    private CommunityRule()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static CommunityRule Create(Guid communityId, string title, string description, int order)
    {
        var entity = new CommunityRule();
        entity.CommunityId = communityId;
        entity.Title = title;
        entity.Description = description;
        entity.Order = order;
        return entity;
    }

    public void Update(string title, string description, int order)
    {
        Title = title;
        Description = description;
        Order = order;
    }
}
