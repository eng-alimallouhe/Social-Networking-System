using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Queries.GetSocialLinks;

public sealed record GetSocialLinksQuery() : IQuery<SocialLinksDto>;