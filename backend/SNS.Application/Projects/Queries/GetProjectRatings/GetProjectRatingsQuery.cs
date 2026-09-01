using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.Projects.Queries.GetProjectRatings;

public sealed record GetProjectRatingsQuery(
    Guid ProjectId,
    int Page,
    int PageSize
) : IQuery<Paged<ProjectRatingDto>>;
