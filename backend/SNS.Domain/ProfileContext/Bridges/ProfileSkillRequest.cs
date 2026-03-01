using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Preferences.Enums;

namespace SNS.Domain.ProfileContext.Bridges;

public class ProfileSkillRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid JoinerId { get; set; }
    public Guid SkillRequestId { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    public ProficiencyLevel Level { get; set; }

    // Navigation

    public ProfileSkillRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}
