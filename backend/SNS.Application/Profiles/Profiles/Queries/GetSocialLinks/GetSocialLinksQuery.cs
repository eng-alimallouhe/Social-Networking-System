using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Queries.GetSocialLinks;

/// <summary>
/// Represents a query to retrieve social media and web links for the authenticated user's profile.
/// </summary>
public sealed record GetSocialLinksQuery() : IQuery<SocialLinksDto>;