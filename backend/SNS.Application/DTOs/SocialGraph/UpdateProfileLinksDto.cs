using SNS.Domain.SocialGraph;

namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// update the social media links of a profile.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer without exposing
/// domain entities or internal business logic.
/// 
/// It is typically used in commands to modify external profile links.
/// </summary>
public class UpdateProfileLinksDto
{
    /// <summary>
    /// Gets or sets the GitHub profile URL.
    /// 
    /// Optional. This value is used to direct visitors to the user's code repositories.
    /// </summary>
    public string? GitHubUrl { get; set; }

    /// <summary>
    /// Gets or sets the LinkedIn profile URL.
    /// 
    /// Optional. This value is used to direct visitors to the user's professional network.
    /// </summary>
    public string? LinkedInUrl { get; set; }

    /// <summary>
    /// Gets or sets the X (formerly Twitter) profile URL.
    /// 
    /// Optional. This value is used to direct visitors to the user's microblogging feed.
    /// </summary>
    public string? XUrl { get; set; }

    /// <summary>
    /// Gets or sets the Facebook profile URL.
    /// 
    /// Optional. This value is used to direct visitors to the user's social page.
    /// </summary>
    public string? FacebookUrl { get; set; }

    /// <summary>
    /// Gets or sets the personal website URL.
    /// 
    /// Optional. This value is used to direct visitors to the user's portfolio or blog.
    /// </summary>
    public string? Website { get; set; }
}