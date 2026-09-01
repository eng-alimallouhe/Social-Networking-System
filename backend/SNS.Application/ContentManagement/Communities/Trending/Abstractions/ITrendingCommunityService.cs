using SNS.Application.ContentManagement.Communities.Communities.Contracts;

namespace SNS.Application.ContentManagement.Communities.Trending.Abstractions;

/// <summary>
/// Provides methods for tracking community activity scores and retrieving trending communities.
/// </summary>
public interface ITrendingCommunityService
{
    /// <summary>
    /// Records activity score boost for a community in Redis sorted set.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="scoreBoost">The score increment.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task TrackActivityAsync(Guid communityId, double scoreBoost, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves top trending communities ordered by score.
    /// </summary>
    /// <param name="count">The maximum number of trending communities to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="CommunitySummaryDto"/> in rank order.</returns>
    Task<List<CommunitySummaryDto>> GetTrendingCommunitiesAsync(int count = 10, CancellationToken cancellationToken = default);
}
