namespace SNS.Application.Discussions.Problems.ProblemTopics.Contracts;

/// <summary>
/// Represents an AI-classified topic associated with a discussion problem.
/// </summary>
/// <param name="TopicId">The unique identifier of the topic.</param>
/// <param name="Name">The display name or slug of the topic.</param>
/// <param name="Confidence">The AI model confidence score, if available.</param>
public sealed record ProblemTopicDto(
    Guid TopicId,
    string Name,
    float? Confidence
);
