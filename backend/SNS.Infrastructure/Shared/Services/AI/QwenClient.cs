using System.Net.Http.Json;
using System.Text.Json;
using SNS.Application.Shared.Abstractions.AI;
using SNS.Application.Shared.Contracts.AI;
using Microsoft.Extensions.Options;

namespace SNS.Infrastructure.Shared.Services.AI;

public sealed class QwenClient(
    HttpClient httpClient,
    IOptions<QwenOptions> options) : IQwenClient
{
    private readonly QwenOptions _options = options.Value;

    public async Task<IReadOnlyList<DetectedTopic>> ClassifyTopicsAsync(
        string content,
        IReadOnlyList<string> availableTopicSlugs,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(content) ||
            availableTopicSlugs.Count == 0)
        {
            return [];
        }

        var prompt = BuildPrompt(content, availableTopicSlugs);

        var request = new
        {
            model = _options.Model,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            temperature = _options.Temperature,
            max_tokens = _options.MaxTokens,
            chat_template_kwargs = new
            {
                enable_thinking = false
            }
        };

        using var response = await httpClient.PostAsJsonAsync(
            "/v1/chat/completions",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<QwenChatResponse>(
            cancellationToken);

        if (result?.Choices is null || result.Choices.Count == 0)
        {
            return [];
        }

        var contentJson = result.Choices[0].Message.Content;

        if (string.IsNullOrWhiteSpace(contentJson))
        {
            return [];
        }

        return ParseTopics(contentJson, availableTopicSlugs);
    }

    private static string BuildPrompt(
        string content,
        IReadOnlyList<string> availableTopicSlugs)
    {
        var topics = string.Join(", ", availableTopicSlugs);

        return $$"""
            You are a topic classifier.

            Select only relevant topics from the provided list.

            STRICT RULES:
            - You MUST select topics only from the provided list.
            - You MUST NOT create new topics.
            - You MUST NOT rename or modify topic slugs.
            - Return only clearly relevant topics.
            - If no topic is relevant, return an empty topics array.
            - Return ONLY valid JSON.
            - Do not include markdown or explanations.

            Available topics:
            [{{topics}}]

            Content:
            {{content}}

            Return exactly this JSON structure:
            {
              "topics": [
                {
                  "slug": "topic-slug",
                  "confidence": 0.95
                }
              ]
            }
            """;
    }

    private static IReadOnlyList<DetectedTopic> ParseTopics(
        string responseContent,
        IReadOnlyList<string> availableTopicSlugs)
    {
        try
        {
            var result = JsonSerializer.Deserialize<QwenTopicResponse>(
                responseContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result?.Topics is null)
            {
                return [];
            }

            var availableTopics = availableTopicSlugs.ToHashSet(
                StringComparer.OrdinalIgnoreCase);

            return result.Topics
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.Slug) &&
                    availableTopics.Contains(x.Slug))
                .Select(x => new DetectedTopic(
                    x.Slug,
                    Math.Clamp(x.Confidence, 0f, 1f)))
                .ToList();
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private sealed record QwenChatResponse(
        List<QwenChoice> Choices);

    private sealed record QwenChoice(
        QwenMessage Message);

    private sealed record QwenMessage(
        string Content);

    private sealed record QwenTopicResponse(
        List<QwenTopic> Topics);

    private sealed record QwenTopic(
        string Slug,
        float Confidence);
}