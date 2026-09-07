using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
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
/// <param name="Author">Snapshot overview of the problem author using shared ProfileSnapshotDto.</param>
/// <param name="Community">Optional snapshot overview of the community if published to one.</param>
/// <param name="UpvotesCount">The count of positive votes.</param>
/// <param name="DownvotesCount">The count of negative votes.</param>
/// <param name="SolutionsCount">The count of submitted solutions.</param>
/// <param name="IsUpVotedByCurrentUser">True if the current user has upvoted this problem.</param>
/// <param name="IsDownVotedByCurrentUser">True if the current user has downvoted this problem.</param>
/// <param name="Tags">The list of associated tag names.</param>
/// <param name="Topics">The list of associated topic names.</param>
/// <param name="CreatedAt">The timestamp when the problem was created.</param>
/// <param name="ContentBlocks">The ordered list of structured content blocks representing the problem content.</param>
public sealed record ProblemSummaryDto(
    Guid Id,
    string Title,
    ProblemStatus Status,
    DifficultyLevel Level,
    ProfileSnapshotDto Author,
    CommunitySnapshotDto? Community,
    int UpvotesCount,
    int DownvotesCount,
    int SolutionsCount,
    bool IsUpVotedByCurrentUser,
    bool IsDownVotedByCurrentUser,
    List<string> Tags,
    List<string> Topics,
    DateTime CreatedAt,
    List<ProblemContentBlockDto> ContentBlocks
);
