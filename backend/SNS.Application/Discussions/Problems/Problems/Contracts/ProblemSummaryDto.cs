using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Application.Discussions.Problems.Problems.Contracts;

/// <summary>
/// Represents summary discussion problem overview information for search, feeds, and list views.
/// </summary>
/// <param name="Id">The unique identifier of the problem.</param>
/// <param name="Title">The problem title.</param>
/// <param name="Status">The lifecycle status of the problem.</param>
/// <param name="Level">The difficulty level of the problem.</param>
/// <param name="AuthorId">The unique identifier of the author profile.</param>
/// <param name="AuthorName">The display name of the author.</param>
/// <param name="AuthorProfilePictureUrl">The resolved public avatar URL of the author.</param>
/// <param name="UpvotesCount">The count of positive votes.</param>
/// <param name="SolutionsCount">The count of submitted solutions.</param>
/// <param name="Tags">The list of associated tag names.</param>
/// <param name="Topics">The list of associated topic names.</param>
/// <param name="CreatedAt">The timestamp when the problem was created.</param>
/// <param name="ContentBlocks">The ordered list of structured content blocks representing the problem content.</param>
public sealed record ProblemSummaryDto(
    Guid Id,
    string Title,
    ProblemStatus Status,
    DifficultyLevel Level,
    Guid AuthorId,
    string AuthorName,
    string? AuthorProfilePictureUrl,
    int UpvotesCount,
    int SolutionsCount,
    List<string> Tags,
    List<string> Topics,
    DateTime CreatedAt,
    List<ProblemContentBlockDto> ContentBlocks
);
