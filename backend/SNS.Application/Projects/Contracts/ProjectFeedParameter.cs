namespace SNS.Application.Projects.Contracts;

public sealed record ProjectFeedParameter(
    Guid ProfileId,
    List<string> Skills,
    List<ProjectTagSnapshot> Tags,
    List<Guid> ExcludedProjectsIds,
    List<Guid> ExcludedProfilesIds,
    List<Guid> FollowedProfilesIds,
    int FeedSize
);

public sealed record ProjectTagSnapshot(
    Guid TagId,
    double Score
);
