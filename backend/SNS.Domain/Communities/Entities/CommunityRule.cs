using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Communities.Entities;

public class CommunityRule : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Community) → Many(Rules)
    public Guid CommunityId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }

    // Navigation Properties (Required)
    public Community Community { get; set; } = null!;

    public CommunityRule()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}