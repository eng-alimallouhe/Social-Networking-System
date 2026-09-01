using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.Projects.Queries.GetProjectMedia;

public sealed record GetProjectMediaQuery(
    Guid ProjectId,
    int Page,
    int PageSize
) : IQuery<Paged<ProjectMediaDto>>;
