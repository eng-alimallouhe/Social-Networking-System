using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Application.Discussions.Problems.Problems.Contracts;

/// <summary>
/// Represents full details of a discussion problem including author, community, content blocks, metrics, and relationship state.
/// </summary>
/// <param name="Id">The unique identifier of the problem.</param>
/// <param name="Title">The problem title.</param>
/// <param name="Status">The lifecycle status.</param>
/// <param name="Level">The difficulty level.</param>
/// <param name="CreatedAt">The creation timestamp.</param>
/// <param name="UpdatedAt">The last update timestamp.</param>
/// <param name="Author">Snapshot overview of the problem author.</param>
/// <param name="Community">Optional snapshot overview of the community.</param>
/// <param name="ContentBlocks">The ordered list of content blocks.</param>
/// <param name="Tags">The list of associated tags.</param>
/// <param name="Topics">The list of AI-classified topics.</param>
/// <param name="UpvotesCount">The count of upvotes.</param>
/// <param name="DownvotesCount">The count of downvotes.</param>
/// <param name="SolutionsCount">The count of solutions.</param>
/// <param name="ViewsCount">The total views recorded.</param>
/// <param name="CurrentUserVote">The vote type cast by the requesting user, if any.</param>
public sealed record ProblemDetailsDto(
    Guid Id,
    string Title,
    ProblemStatus Status,
    DifficultyLevel Level,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    ProfileSnapshotDto Author,
    CommunitySnapshotDto? Community,
    List<ProblemContentBlockDto> ContentBlocks,
    List<string> Tags,
    List<string> Topics,
    int UpvotesCount,
    int DownvotesCount,
    int SolutionsCount,
    int ViewsCount,
    VoteType? CurrentUserVote
);