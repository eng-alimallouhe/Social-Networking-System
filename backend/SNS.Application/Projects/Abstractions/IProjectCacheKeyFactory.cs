namespace SNS.Application.Projects.Abstractions;

public interface IProjectCacheKeyFactory
{
    string GetProjectProfileFeedKey(Guid profileId);
}
