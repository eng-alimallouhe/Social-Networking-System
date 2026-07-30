namespace SNS.Application.Shared.Contracts.AI;

/// <summary>
/// Represents an AI-detected topic model containing topic slug and confidence score.
/// </summary>
/// <param name="slug">The unique topic identifier slug.</param>
/// <param name="Confidence">The classification confidence score assigned to the detected topic.</param>
public sealed record DetectedTopic(
    string slug,
    float Confidence
);