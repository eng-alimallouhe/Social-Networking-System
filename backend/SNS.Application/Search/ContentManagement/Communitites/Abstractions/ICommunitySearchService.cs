using SNS.Application.Search.ContentManagement.Communitites.Queries;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.ContentManagement.Communitites.Abstractions;

public interface ICommunitySearchService
{
    Task<SearchResult<CommunityDocument>> SearchCommunitiesAsync(CommunitySearchQuery query, CancellationToken cancellationToken = default);

    Task<SearchResult<CommunityDocument>> GetSuggestedCommunities( CancellationToken cancellationToken = default);

    Task<SearchResult<CommunityDocument>> GetCommunitiesByIds(
        List<string> communityIds, 
        int count = 10,
        CancellationToken cancellationToken = default);

    Task<Result> UpsertCommunityAsync(CommunityDocument communityDocument, CancellationToken cancellationToken = default);
     Task<Result> DeleteCommunityAsync(string communityId, CancellationToken cancellationToken = default);
}
