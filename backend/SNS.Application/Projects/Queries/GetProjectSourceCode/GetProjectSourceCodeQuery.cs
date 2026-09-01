using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Projects.ValueObjects;

namespace SNS.Application.Projects.Queries.GetProjectSourceCode;

public sealed record GetProjectSourceCodeQuery(Guid ProjectId) : IQuery<List<FileNode>>;
