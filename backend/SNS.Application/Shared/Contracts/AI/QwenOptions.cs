namespace SNS.Application.Shared.Contracts.AI;

public sealed class QwenOptions
{
    public const string SectionName = "AI:Qwen";

    public string BaseUrl { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public int MaxTokens { get; init; } = 300;

    public float Temperature { get; init; } = 0.1f;
}