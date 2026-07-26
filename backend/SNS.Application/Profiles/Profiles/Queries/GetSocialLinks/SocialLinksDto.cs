namespace SNS.Application.Profiles.Profiles.Queries.GetSocialLinks;

public sealed record SocialLinksDto(
    string? GitHubUrl,
    string? LinkedInUrl,
    string? FacebookUrl,
    string? XUrl,
    string? Website);
