namespace SNS.Application.Shared.Contracts.AI;

public sealed record DetectedTopic(
    string slug,
    float Confidence
);