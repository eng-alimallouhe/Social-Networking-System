using SNS.Application.Search.ContentManagement.Posts.Queries;

namespace SNS.Application.ContentManagement.Posts.Abstractions;

public interface IFeedBackgroundService 
{
    Task ComputeAndCacheUserFeedAsync(Guid profileId, FeedRequestParameter feedParams);
}