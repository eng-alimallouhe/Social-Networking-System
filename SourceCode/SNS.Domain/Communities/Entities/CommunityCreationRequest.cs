using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Communities.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Communities.Entities;


public class CommunityCreationRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(CreationRequests)
    public Guid SubmitterProfileId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RulesText { get; set; } = string.Empty;
    public ModerationPolicy Policy { get; set; }
    public CommunityType Type { get; set; }
    public CommunityStatus Status { get; set; }

    // Timestamp
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }

    public string ReviewNotes { get; set; } = string.Empty;

    // Navigation Properties (Required)
    public Profile SubmitterProfile { get; set; } = null!;

    public CommunityCreationRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        RequestedAt = DateTime.UtcNow;
    }
}