using SNS.Application.DTOs.Education.Responses;
using SNS.Application.DTOs.Preferences;
using SNS.Application.DTOs.ProfileContext;

namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// provide a comprehensive view of a user's profile.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client for the main profile page display,
/// aggregating data from multiple domain contexts (Core, Education, Preferences).
/// 
/// It is typically used in responses when viewing a specific user profile.
/// </summary>
public class ProfileDetailsDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the full name of the profile owner.
    /// 
    /// This value is used as the primary display name on the profile page.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the biography text.
    /// 
    /// Optional. This value is used to provide a short introduction about the user.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the URL of the profile picture.
    /// 
    /// Optional. This value is used to render the user's avatar.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL of the cover image.
    /// 
    /// Optional. This value is used to render the profile header background.
    /// </summary>
    public string? CoverImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the primary specialization.
    /// 
    /// Optional. This value is used to display the user's professional title.
    /// </summary>
    public string? Specialization { get; set; }

    /// <summary>
    /// Gets or sets the total number of followers.
    /// 
    /// This value is used to indicate the user's audience size.
    /// </summary>
    public int FollowersCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of profiles this user is following.
    /// </summary>
    public int FollowingCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of times the profile has been viewed.
    /// </summary>
    public int ViewsCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of projects associated with the profile.
    /// </summary>
    public int ProjectsCount { get; set; }


    /// <summary>
    /// Gets or sets the total number of contributors across all projects.
    /// </summary>
    public int ProjectContributorsCount { get; set; }


    /// <summary>
    /// Gets or sets the total number of solutions published by the user.
    /// </summary>
    public int SolutionsCount { get; set; }

    /// <summary>
    /// Gets or sets the list of professional skills.
    /// 
    /// This collection is used to display the user's technical competencies.
    /// </summary>
    public List<ProfileSkillDto> Skills { get; set; } = new List<ProfileSkillDto>();

    /// <summary>
    /// Gets or sets the list of personal interests.
    /// 
    /// This collection is used to display what the user is passionate about.
    /// </summary>
    public List<InterestSummaryDto> Interests { get; set; } = new();

    
    /// <summary>
    /// Gets or sets the city of residence.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the associated university details.
    /// 
    /// Optional. This value is used to display the user's academic background.
    /// </summary>
    public UniversityLookupDto? University { get; set; }

    /// <summary>
    /// Gets or sets the associated faculty details.
    /// 
    /// Optional. This value is used to display the user's specific field of study.
    /// </summary>
    public FacultyLookupDto? Faculty { get; set; }

    /// <summary>
    /// Gets or sets the GitHub profile URL.
    /// </summary>
    public string? GitHubUrl { get; set; }

    /// <summary>
    /// Gets or sets the LinkedIn profile URL.
    /// </summary>
    public string? LinkedInUrl { get; set; }

    /// <summary>
    /// Gets or sets the X (formerly Twitter) profile URL.
    /// </summary>
    public string? XUrl { get; set; }

    /// <summary>
    /// Gets or sets the Facebook profile URL.
    /// </summary>
    public string? FacebookUrl { get; set; }

    /// <summary>
    /// Gets or sets the personal website URL.
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Indicates whether the current viewer is following this profile.
    /// 
    /// This value is used to toggle the "Follow/Unfollow" button state.
    /// </summary>
    public bool IsFollowedByViewer { get; set; }

    /// <summary>
    /// Indicates whether the profile is blocked by the viewer.
    /// 
    /// This value is used to manage UI visibility and block actions.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Indicates whether the profile owner has blocked the viewer.
    /// 
    /// This value is used to restrict access to profile content.
    /// </summary>
    public bool IsBlockingViewer { get; set; }

    /// <summary>
    /// Gets or sets the detailed profile view counter.
    /// 
    /// This duplicate property (see ViewsCount) serves as a specific metric for analytics.
    /// </summary>
    public int ProfileViews { get; set; }
}