using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.Profiles.Profiles.Queries.GetViewedProfiles;

public sealed record GetViewedProfilesQuery(
    int PageSize = 10, 
    int CurrentPage = 1
    ) : IQuery<Paged<ProfileViewDto>>;
