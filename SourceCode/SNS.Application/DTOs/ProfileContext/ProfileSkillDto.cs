using SNS.Domain.Preferences.Enums;

namespace SNS.Application.DTOs.ProfileContext;

public class ProfileSkillDto
{
    public Guid Id { get; set; }
    public Guid SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public ProficiencyLevel ProficiencyLevel { get; set; }
}
