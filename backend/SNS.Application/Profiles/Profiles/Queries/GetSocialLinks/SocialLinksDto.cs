namespace SNS.Application.Profiles.Profiles.Queries.GetSocialLinks;

/// <summary>
/// Represents data transfer object containing user external social media profile URLs.
/// </summary>
/// <param name="GitHubUrl">Optional URL to GitHub profile.</param>
/// <param name="LinkedInUrl">Optional URL to LinkedIn profile.</param>
/// <param name="FacebookUrl">Optional URL to Facebook profile.</param>
/// <param name="XUrl">Optional URL to X (formerly Twitter) profile.</param>
/// <param name="Website">Optional URL to personal blog or website.</param>
public sealed record SocialLinksDto(
    string? GitHubUrl,
    string? LinkedInUrl,
    string? FacebookUrl,
    string? XUrl,
    string? Website);

