using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Communities.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Communities.Entities;

public class CommunityJoinRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Community) → Many(JoinRequests)
    public Guid CommunityId { get; set; }

    // Foreign Key: One(Profile) → Many(JoinRequests)
    public Guid SubmitterId { get; set; }

    public JoinRequestStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    // Navigation Properties (Required)
    public Community Community { get; set; } = null!;
    public Profile Submitter { get; set; } = null!;


    public CommunityJoinRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = JoinRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }
}
