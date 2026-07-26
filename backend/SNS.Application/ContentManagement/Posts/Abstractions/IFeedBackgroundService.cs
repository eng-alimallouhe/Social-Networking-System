namespace SNS.Application.ContentManagement.Posts.Abstractions;

public interface IFeedBackgroundService 
{
    Task ComputeAndCacheUserFeedAsync(Guid profileId);
}