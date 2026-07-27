namespace SNS.Application.Search.ContentManagement.Posts.Queries;

public sealed record FeedRequestParameter(
    Guid ProfileId,
    List<string> Skills,
    List<Guid> ExcludedPostsIds,
    List<Guid> ExcludedProfilesIds,
    List<Guid> CommunitiesIds,
    List<Guid> FollowedProfilesIds,
    DateTime StartDate,
    List<ProfileTopicSnapshot> Topics,
    List<ProfileTagSnapshot> Tags,
    int FeedSize);


public sealed record ProfileTagSnapshot(
    Guid TagId,
    double Score
);

public sealed record ProfileTopicSnapshot(
    Guid TopicId,
    double Score
);