using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Projects.Queries.GetProjectMilestones;

public sealed record GetProjectMilestonesQuery(Guid ProjectId) : IQuery<List<ProjectMilestoneDto>>;
