using MediatR;
using SNS.Application.Search.ContentManagement.Communitites.Queries;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;
using SNS.Domain.Search.Documents;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Search.ContentManagement.Communitites.Queries.GetCommunitiesSearch;

/// <summary>
/// Represents a search query to search for community documents in the search index using specified filter parameters.
/// </summary>
/// <param name="Parameters">The search filter, sorting, and pagination parameters for communities.</param>
public sealed record GetCommunitiesSearchQuery(CommunitySearchQuery Parameters)
: IQuery<SearchResult<CommunityDocument>>;

