using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Shared.Results;

namespace SNS.Application.ContentManagement.Posts.Posts.Abstractions;

public interface IPostCacheService
{
    Task<Result> SetProfileFeedAsync(
        Guid profileId, 
        List<FeedItemModel> feedItems, 
        CancellationToken cancellationToken = default);

    Task<List<FeedItemModel>> GetProfileFeed(
        Guid profileId,
        long start,
        long stop,
        CancellationToken cancellationToken = default);

    Task<bool> TryLockFeedBuildingAsync(Guid profileId);
    
    Task UnlockFeedBuildingAsync(Guid profileId);
}
