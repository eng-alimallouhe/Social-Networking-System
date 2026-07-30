using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfilePictureUrl;

/// <summary>
/// Represents a query to retrieve a temporary public URL for the authenticated user's profile picture.
/// </summary>
public sealed record GetProfilePictureUrlQuery(): IQuery<string>;