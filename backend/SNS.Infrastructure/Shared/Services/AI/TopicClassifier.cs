using SNS.Application.ContentManagement.Posts.Contracts;
using SNS.Application.Shared.Abstractions.AI;
using SNS.Application.Shared.Contracts.AI;

namespace SNS.Infrastructure.Shared.Services.AI;

public sealed class TopicClassifier
    : ITopicClassifier
{
#warning Insure implement the service using the AI model when it be completed
    public Task<IReadOnlyList<DetectedTopic>> DetectTopicsAsync(PostAnalysisRequest request)
    {
        return Task.FromResult<IReadOnlyList<DetectedTopic>>(
        [
            new("programming", (float)0.95),
            new("asp-net", (float)0.89)
        ]);
    }
}