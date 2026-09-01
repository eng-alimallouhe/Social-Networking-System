using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Discussions.Problems.Contracts;
using SNS.Application.Shared.Contracts.AI;

namespace SNS.Application.Shared.Abstractions.AI;

/// <summary>
/// Provides AI-powered topic detection and classification for content.
/// </summary>
public interface ITopicClassifier
{
    /// <summary>
    /// Detects relevant topics and confidence scores for a post.
    /// </summary>
    /// <param name="request">The post analysis payload.</param>
    /// <returns>A list of detected topics with confidence scores.</returns>
    Task<IReadOnlyList<DetectedTopic>> DetectTopicsAsync(PostAnalysisRequest request);

    /// <summary>
    /// Detects relevant topics and confidence scores for a problem discussion.
    /// </summary>
    /// <param name="request">The problem analysis payload.</param>
    /// <returns>A list of detected topics with confidence scores.</returns>
    Task<IReadOnlyList<DetectedTopic>> DetectTopicsAsync(ProblemAnalysisRequest request);
}