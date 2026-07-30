using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfileViewers;

/// <summary>
/// Represents a query to retrieve a paged list of profiles that viewed the authenticated user's profile.
/// </summary>
/// <param name="PageSize">The maximum number of viewer records to return per page.</param>
/// <param name="CurrentPage">The page index for pagination (1-based).</param>
public sealed record GetProfileViewersQuery(
    int PageSize = 10,
    int CurrentPage = 1
) : IQuery<Paged<ProfileViewDto>>;

