namespace SNS.Application.ContentManagement.Posts.Posts.Contracts;

public sealed record FeedCandidate(
    Guid PostId,
    double Score
);
