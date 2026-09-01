namespace SNS.Application.ContentManagement.Posts.Posts.Abstractions;

public interface IPostCacheKeyFactory
{
    string GetProfileFeedKey(Guid profileId);
    string GetCelebrityPostKey(Guid publisherId);
    string GetFeedBuildingKey(Guid profileId);
}
