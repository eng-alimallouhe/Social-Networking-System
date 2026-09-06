using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Search.ContentManagement.Posts.Queries;

namespace SNS.Application.ContentManagement.Posts.Posts.Abstractions;

public interface IFeedFallbackService
{
    Task<List<PostOverviewDto>> GetFallbackFeedAsync(
        FeedRequestParameter parameter, 
        int pageSize = 30, 
        CancellationToken cancellationToken = default);
}
