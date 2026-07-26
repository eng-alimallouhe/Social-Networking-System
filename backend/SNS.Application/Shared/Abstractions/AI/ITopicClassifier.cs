using SNS.Application.ContentManagement.Posts.Contracts;
using SNS.Application.Shared.Contracts.AI;

namespace SNS.Application.Shared.Abstractions.AI;

public interface ITopicClassifier
{
    Task<IReadOnlyList<DetectedTopic>> DetectTopicsAsync(PostAnalysisRequest request);
}