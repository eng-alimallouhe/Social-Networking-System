using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Discussions.Problems.Contracts;
using SNS.Application.Shared.Abstractions.AI;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Contracts.AI;

namespace SNS.Infrastructure.Shared.Services.AI;

public sealed class TopicClassifier(
    IApplicationDbContext dbContext,
    IQwenClient qwenClient) : ITopicClassifier
{
    public async Task<IReadOnlyList<DetectedTopic>> DetectTopicsAsync(
        PostAnalysisRequest request)
    {
        var content = request.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            return [];
        }

        var availableTopicSlugs = await GetAvailableTopicSlugsAsync();

        return await qwenClient.ClassifyTopicsAsync(
            content,
            availableTopicSlugs);
    }

    public async Task<IReadOnlyList<DetectedTopic>> DetectTopicsAsync(
        ProblemAnalysisRequest request)
    {
        var content = BuildProblemContent(request);

        if (string.IsNullOrWhiteSpace(content))
        {
            return [];
        }

        var availableTopicSlugs = await GetAvailableTopicSlugsAsync();

        return await qwenClient.ClassifyTopicsAsync(
            content,
            availableTopicSlugs);
    }

    private async Task<IReadOnlyList<string>> GetAvailableTopicSlugsAsync()
    {
        return await dbContext.Topics
            .AsNoTracking()
            .Select(topic => topic.Name)
            .ToListAsync();
    }

    private static string BuildProblemContent(
        ProblemAnalysisRequest request)
    {
        var sections = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            sections.Add($"Title:\n{request.Title}");
        }

        if (request.TextBlocks.Count > 0)
        {
            sections.Add(
                $"Text:\n{string.Join("\n", request.TextBlocks)}");
        }

        if (request.Codes.Count > 0)
        {
            sections.Add(
                $"Code:\n{string.Join("\n", request.Codes)}");
        }

        return string.Join("\n\n", sections);
    }
}