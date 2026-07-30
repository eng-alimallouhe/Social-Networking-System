using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Projects.Queries.GetProjectsSearch;

/// <summary>
/// Represents a search query to search project documents in the search index using specified filter criteria.
/// </summary>
/// <param name="Parameters">The search filter, sorting, and pagination parameters for projects.</param>
public sealed record GetProjectsSearchQuery(ProjectSearchQuery Parameters)
: IQuery<SearchResult<ProjectDocument>>;

