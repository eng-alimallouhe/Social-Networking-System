namespace SNS.Application.ContentManagement.Posts.Abstractions;

public interface IPostCacheKeyFactory
{
    string GetUserPostsKey(Guid userId);
    string GetPostKey(Guid postId);
}
