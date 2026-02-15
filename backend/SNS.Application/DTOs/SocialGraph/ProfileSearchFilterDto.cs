namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// encapsulate the criteria for searching and filtering user profiles.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer, allowing users to find
/// others based on shared interests, skills, location, or education.
/// 
/// It is typically used in the search query request.
/// </summary>
public class ProfileSearchFilterDto
{
    /// <summary>
    /// Gets or sets the partial or full name to search for.
    /// 
    /// Optional. This value is used to perform a text-based match against the profile owner's name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the list of interest identifiers to filter by.
    /// 
    /// Optional. This value is used to include only profiles that have listed these specific interests.
    /// </summary>
    public List<Guid>? InterestIds { get; set; }

    /// <summary>
    /// Gets or sets the list of skill identifiers to filter by.
    /// 
    /// Optional. This value is used to include only profiles that possess these specific professional skills.
    /// </summary>
    public List<Guid>? SkillIds { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the university.
    /// 
    /// Optional. This value is used to filter profiles associated with this specific educational institution.
    /// </summary>
    public Guid? UniversityId { get; set; }

    /// <summary>
    /// Gets or sets the country name.
    /// 
    /// Optional. This value is used to filter profiles based on their geographical location (Country).
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the city name.
    /// 
    /// Optional. This value is used to filter profiles based on their geographical location (City).
    /// </summary>
    public string? City { get; set; }
}