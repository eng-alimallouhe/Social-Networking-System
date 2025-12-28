using SNS.Application.DTOs.Education.Responses;
using SNS.Application.DTOs.Preferences;
using SNS.Application.DTOs.ProfileContext;

namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents full public profile details.
/// </summary>
/// <remarks>
/// Returned when viewing a complete profile page.
/// </remarks>
public class ProfileDetailsDto
{
    /// <summary>
    /// Unique profile identifier.
    /// </summary>
    public Guid ProfileId { get; set; }

    /// <summary>
    /// Full name of the profile owner.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Profile biography.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Profile picture URL.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Cover image URL.
    /// </summary>
    public string? CoverImageUrl { get; set; }

    /// <summary>
    /// Primary specialization.
    /// </summary>
    public string? Specialization { get; set; }

    /// <summary>
    /// Number of followers.
    /// </summary>
    public int FollowersCount { get; set; }

    /// <summary>
    /// Number of followed profiles.
    /// </summary>
    public int FollowingCount { get; set; }

    /// <summary>
    /// Number of profile views.
    /// </summary>
    public int ViewsCount { get; set; }

    /// <summary>
    /// Number of projects associated with the profile.
    /// </summary>
    public int ProjectsCount { get; set; }

    /// <summary>
    /// Number of published solutions.
    /// </summary>
    public int SolutionsCount { get; set; }

    /// <summary>
    /// List of skills.
    /// </summary>
    public List<ProfileSkillDto> Skills { get; set; } = new List<ProfileSkillDto>();

    /// <summary>
    /// List of interests.
    /// </summary>
    public List<InterestSummaryDto> Interests { get; set; } = new();

    /// <summary>
    /// Country of residence.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// City of residence.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// include university name and identifier.
    /// </summary>
    public UniversityLookupDto? University { get; set; }

    /// <summary>
    /// include Faculty name and identifier..
    /// </summary>
    public FacultyLookupDto? Faculty { get; set; }

    /// <summary>
    /// GitHub profile URL.
    /// </summary>
    public string? GitHubUrl { get; set; }

    /// <summary>
    /// LinkedIn profile URL.
    /// </summary>
    public string? LinkedInUrl { get; set; }

    /// <summary>
    /// Twitter (X) profile URL.
    /// </summary>
    public string? XUrl { get; set; }

    /// <summary>
    /// Facebook profile URL.
    /// </summary>
    public string? FacebookUrl { get; set; }

    /// <summary>
    /// Personal website URL.
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Indicates whether the viewer follows this profile.
    /// </summary>
    public bool IsFollowedByViewer { get; set; }

    /// <summary>
    /// Indicates whether the profile is blocked by the viewer.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Indicates whether the profile owner blocks the viewer.
    /// </summary>
    public bool IsBlockingViewer { get; set; }

    /// <summary>
    /// The counter of the profile views 
    /// </summary>
    public int ProfileViews { get; set; }
}
