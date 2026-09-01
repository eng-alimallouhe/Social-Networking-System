using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(Guid ProjectId) : IQuery<ProjectDetailsDto>;
