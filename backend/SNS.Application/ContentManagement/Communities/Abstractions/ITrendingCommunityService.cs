using SNS.Domain.Search.Documents;

namespace SNS.Application.ContentManagement.Communities.Abstractions;

public interface ITrendingCommunityService
{
    Task TrackActivityAsync(Guid communityId, double scoreBoost, CancellationToken cancellationToken = default);

    Task<List<CommunityDocument>> GetTrendingCommunitiesAsync(int count = 10, CancellationToken cancellationToken = default);
}
