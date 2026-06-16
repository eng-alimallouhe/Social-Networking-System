namespace SNS.Application.Search.Discussions.Problems.Queries;

public sealed record ProblemFeedParameter(
    Guid ProfileId,
    List<string> Skills,
    List<Guid> ExcludedProblemsIds,
    List<Guid> ExcludedProfilesIds,
    List<Guid> CommunitiesIds,
    List<Guid> FollowedProfilesIds,
    DateTime StartDate,
    List<string> Topics,
    List<string> Tags,
    int FeedSize);

