using SNS.Application.Shared.Contracts.AI;

namespace SNS.Application.Shared.Abstractions.AI;


public interface IQwenClient
{
    Task<IReadOnlyList<DetectedTopic>> ClassifyTopicsAsync(
        string content,
        IReadOnlyList<string> availableTopicSlugs,
        CancellationToken cancellationToken = default);
}