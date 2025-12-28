namespace SNS.Application.DTOs.Preferences;

/// <summary>
/// Represents a summary of an interest, including its unique identifier and name.
/// </summary>
public class InterestSummaryDto
{
    /// <summary>
    /// Interest identifier.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Interest name.
    /// </summary>
    public string InterestName { get; set; } = string.Empty;
}