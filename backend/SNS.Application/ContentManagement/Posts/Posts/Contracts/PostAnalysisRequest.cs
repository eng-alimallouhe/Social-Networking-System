namespace SNS.Application.ContentManagement.Posts.Posts.Contracts;

public sealed record PostAnalysisRequest(
    string Content,
    List<string> Videos,
    List<string> Images
);
