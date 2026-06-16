namespace SNS.Application.Search.ContentManagement.Posts.Queries;

public sealed record FeedRequestParameter(
    Guid ProfileId,
    List<string> Skills,
    List<Guid> ExcludedPostsIds,
    List<Guid> ExcludedProfilesIds,
    List<Guid> CommunitiesIds,
    List<Guid> FollowedProfilesIds,
    DateTime StartDate,
    List<string> Topics,
    List<string> Tags,
    int FeedSize);
