using SNS.Domain.Preferences.Entities;
using SNS.Domain.Preferences.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.ProfileContext.Bridges;

public class ProfileSkillRequest
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProfileId { get; set; }
    public Guid SkillRequestId { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    public ProficiencyLevel ProficiencyLevel { get; set; }

    // Navigation
    public SkillRequest SkillRequest { get; set; } = null!;
    public Profile Profile { get; set; } = null!;
}
