using SNS.Application.Profiles.SocialGraph.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.Profiles.SocialGraph.Queries.GetProfileFollowings;

public sealed record GetProfileFollowingsQuery(
    Guid ProfileId,
    string? SearchTerm,
    int PageSize = 10,
    int CurrentPage = 1
) : IQuery<Paged<ProfileFollowDto>>;
