using SNS.Application.Search.ContentManagement.Posts.Queries.GetPostsSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.Search.Documents;
using SNS.Shared.Results;

namespace SNS.Application.Search.ContentManagement.Posts.Abstractions;

public interface IPostSearchService
{
    public Task<SearchResult<PostDocument>> SearchAsync(GetPostsSearchQuery query, CancellationToken cancellationToken = default);

    public Task<Result> UpsertPostAsync(PostDocument post, CancellationToken cancellationToken = default);

    public Task<Result> BulkPostsAsync(List<PostDocument> posts, CancellationToken cancellationToken = default);

    public Task<Result> DeletePostAsync(Guid postId, CancellationToken cancellationToken = default);

    public Task<Result> DeletePostsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
}
