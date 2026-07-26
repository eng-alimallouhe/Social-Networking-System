namespace SNS.Application.ContentManagement.Posts.Contracts;

public sealed record FeedCandidate(
    Guid PostId,
    double Score);
