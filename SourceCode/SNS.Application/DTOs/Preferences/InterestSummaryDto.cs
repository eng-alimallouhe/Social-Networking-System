namespace SNS.Application.DTOs.Preferences;

/// <summary>
/// Represents a data transfer object used to
/// provide a lightweight representation of a user interest.
/// 
/// This DTO is designed to transfer data between
/// application boundaries without exposing
/// domain entities or internal business logic.
/// 
/// It is typically used in lookups, lists, or profile displays.
/// </summary>
public class InterestSummaryDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the Interest.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the interest.
    /// 
    /// This value is used to display the interest label to the user (e.g., "Photography", "Coding").
    /// </summary>
    public string Name { get; set; } = string.Empty;
}