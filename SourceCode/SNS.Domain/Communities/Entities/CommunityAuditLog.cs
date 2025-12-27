using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Communities.Entities;

public class CommunityAuditLog : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Community) → Many(Logs)
    public Guid CommunityId { get; set; }

    // Foreign Key: One(Profile) → Many(AuditLogs) == Optional
    public Guid? ProfileId { get; set; }

    public string Action { get; set; } = string.Empty;

    // Timestamp
    public DateTime PerformedAt { get; set; }

    // Navigation Properties (Required)
    public Community Community { get; set; } = null!;

    // Navigation Properties (Optional)
    public Profile? Profile { get; set; }

    public CommunityAuditLog()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        PerformedAt = DateTime.UtcNow;
    }
}
