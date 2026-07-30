using SNS.Application.Profiles.SocialGraph.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.Profiles.SocialGraph.Queries.GetProfileFollowings;

/// <summary>
/// Represents a query to retrieve a paged list of profiles followed by a specified profile.
/// </summary>
/// <param name="ProfileId">The unique identifier of the profile whose followings are being queried.</param>
/// <param name="SearchTerm">Optional search term to filter followed profiles by full name or specialization.</param>
/// <param name="PageSize">The maximum number of following records to return per page.</param>
/// <param name="CurrentPage">The page index for pagination (1-based).</param>
public sealed record GetProfileFollowingsQuery(
    Guid ProfileId,
    string? SearchTerm,
    int PageSize = 10,
    int CurrentPage = 1
) : IQuery<Paged<ProfileFollowDto>>;

