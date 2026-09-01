using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Discussions.Problems.Contracts;
using SNS.Application.Shared.Abstractions.AI;
using SNS.Application.Shared.Contracts.AI;

namespace SNS.Infrastructure.Shared.Services.AI;

public sealed class TopicClassifier
    : ITopicClassifier
{
    public Task<IReadOnlyList<DetectedTopic>> DetectTopicsAsync(PostAnalysisRequest request)
    {
        return Task.FromResult<IReadOnlyList<DetectedTopic>>(
        [
            new("programming", (float)0.95),
            new("asp-net", (float)0.89)
        ]);
    }

    public Task<IReadOnlyList<DetectedTopic>> DetectTopicsAsync(ProblemAnalysisRequest request)
    {
        return Task.FromResult<IReadOnlyList<DetectedTopic>>(
        [
            new("programming", (float)0.95),
            new("asp-net", (float)0.89)
        ]);
    }
}