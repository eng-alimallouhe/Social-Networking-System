using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.Profiles.Profiles.Queries.GetViewedProfiles;

/// <summary>
/// Represents a query to retrieve a paged list of profiles that the authenticated user has viewed.
/// </summary>
/// <param name="PageSize">The maximum number of viewed profile records to return per page.</param>
/// <param name="CurrentPage">The page index for pagination (1-based).</param>
public sealed record GetViewedProfilesQuery(
    int PageSize = 10, 
    int CurrentPage = 1
    ) : IQuery<Paged<ProfileViewDto>>;

