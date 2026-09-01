using SNS.Application.ContentManagement.Communities.Settings.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.ContentManagement.Communities.Settings.Queries.GetCommunitySettings;

/// <summary>
/// Represents a query to retrieve configuration settings for a community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
public sealed record GetCommunitySettingsQuery(Guid CommunityId) : IQuery<CommunitySettingsDto>;
