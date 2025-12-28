using SNS.Domain.SocialGraph;

namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents social and external links associated with a profile.
/// </summary>
/// <remarks>
/// All properties are optional and can be updated independently.
/// </remarks>
public class UpdateProfileLinksDto
{
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
    /// Personal or portfolio website URL.
    /// </summary>
    public string? Website { get; set; }
}
