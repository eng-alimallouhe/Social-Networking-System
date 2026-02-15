using SNS.Domain.Preferences.Enums;

namespace SNS.Application.DTOs.ProfileContext;

/// <summary>
/// Represents a data transfer object used to
/// convey details about a specific skill possessed by a user profile.
/// 
/// This DTO is designed to transfer data between
/// application boundaries without exposing
/// domain entities or internal business logic.
/// 
/// It is typically used in profile queries and responses.
/// </summary>
public class ProfileSkillDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile-skill association.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the skill definition.
    /// </summary>
    public Guid SkillId { get; set; }

    /// <summary>
    /// Gets or sets the name of the skill.
    /// 
    /// This value is used to display the skill label (e.g., "C#", "Leadership") 
    /// without requiring additional lookups.
    /// </summary>
    public string SkillName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the proficiency level of the user in this skill.
    /// 
    /// This value is used to indicate the depth of knowledge (e.g., Beginner, Expert).
    /// </summary>
    public ProficiencyLevel ProficiencyLevel { get; set; }
}