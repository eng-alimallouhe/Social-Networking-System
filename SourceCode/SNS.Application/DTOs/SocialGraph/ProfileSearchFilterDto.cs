namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents filtering criteria for profile search operations.
/// </summary>
public class ProfileSearchFilterDto
{
    /// <summary>
    /// Partial or full name of the profile owner.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// List of interest identifiers to filter by.
    /// </summary>
    public List<Guid>? InterestIds { get; set; }

    /// <summary>
    /// List of skill identifiers to filter by.
    /// </summary>
    public List<Guid>? SkillIds { get; set; }

    /// <summary>
    /// University identifier filter.
    /// </summary>
    public Guid? UniversityId { get; set; }


    /// <summary>
    /// Country filter.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// City filter.
    /// </summary>
    public string? City { get; set; }
}
