namespace SNS.Application.Discussions.Problems.Contracts;

/// <summary>
/// Represents the payload for analyzing a discussion problem and detecting relevant topics using AI.
/// </summary>
/// <param name="Title">The problem title.</param>
/// <param name="Codes">Content extracted from problem content blocks that represent code.</param>
/// <param name="TextBlocks">Text extracted from problem content blocks that represent textual content.</param>
/// <param name="Videos">Public URLs for video media associated with the problem.</param>
/// <param name="Images">Public URLs for image media associated with the problem.</param>
public sealed record ProblemAnalysisRequest(
    string Title,
    List<string> Codes,
    List<string> TextBlocks,
    List<string> Videos,
    List<string> Images
);
