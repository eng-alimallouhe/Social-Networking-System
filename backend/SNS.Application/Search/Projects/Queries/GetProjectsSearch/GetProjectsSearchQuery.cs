using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Projects.Queries.GetProjectsSearch;

public sealed record GetProjectsSearchQuery(ProjectSearchQuery Parameters)
: IQuery<SearchResult<ProjectDocument>>;
