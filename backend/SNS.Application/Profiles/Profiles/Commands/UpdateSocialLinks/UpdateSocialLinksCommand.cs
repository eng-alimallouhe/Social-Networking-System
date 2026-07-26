using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Commands.UpdateSocialLinks;

public sealed record UpdateSocialLinksCommand(
    string FaceBookUrl,
    string LinkedInUrl,
    string GitHubUrl,
    string XUrl, 
    string Website
) : ICommand;
