using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Commands.UpdateSocialLinks;

/// <summary>
/// Represents a command to update the social media and web links on the authenticated user's profile.
/// </summary>
/// <param name="FaceBookUrl">The Facebook profile URL.</param>
/// <param name="LinkedInUrl">The LinkedIn profile URL.</param>
/// <param name="GitHubUrl">The GitHub profile URL.</param>
/// <param name="XUrl">The X (Twitter) profile URL.</param>
/// <param name="Website">The personal or organization website URL.</param>
public sealed record UpdateSocialLinksCommand(
    string FaceBookUrl,
    string LinkedInUrl,
    string GitHubUrl,
    string XUrl, 
    string Website
) : ICommand;

