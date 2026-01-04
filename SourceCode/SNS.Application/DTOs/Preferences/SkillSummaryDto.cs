namespace SNS.Application.DTOs.Preferences;

/// <summary>
/// Represents a data transfer object used to
/// provide a lightweight representation of a professional skill.
/// 
/// This DTO is designed to transfer data between
/// application boundaries without exposing
/// domain entities or internal business logic.
/// 
/// It is typically used in lookups, lists, or profile displays.
/// </summary>
public class SkillSummaryDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the Skill.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the skill.
    /// 
    /// This value is used to display the skill label to the user (e.g., "C#", "Project Management").
    /// </summary>
    public string Name { get; set; } = string.Empty;
}