using MediatR;
using SNS.Application.Search.ContentManagement.Communitites.Queries;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;
using SNS.Domain.Search.Documents;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Search.ContentManagement.Communitites.Queries.GetCommunitiesSearch;

public sealed record GetCommunitiesSearchQuery(CommunitySearchQuery Parameters)
: IQuery<SearchResult<CommunityDocument>>;
