using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Domain.Discussions.Solutions.Enums;

namespace SNS.Application.Discussions.Solutions.Solutions.Contracts;

/// <summary>
/// Represents complete details of a proposed solution including content blocks and current user vote status.
/// </summary>
/// <param name="Id">The unique identifier of the solution.</param>
/// <param name="ProblemId">The associated problem identifier.</param>
/// <param name="Status">The solution status.</param>
/// <param name="CreatedAt">The timestamp when the solution was submitted.</param>
/// <param name="UpdatedAt">The timestamp of the last update.</param>
/// <param name="Author">Snapshot of the solution author profile.</param>
/// <param name="ContentBlocks">The ordered list of structured content blocks.</param>
/// <param name="UpvotesCount">The count of upvotes.</param>
/// <param name="DownvotesCount">The count of downvotes.</param>
/// <param name="DiscussionsCount">The count of threaded discussion comments.</param>
/// <param name="CurrentUserVote">The vote cast by the authenticated user, if any.</param>
public sealed record SolutionDetailsDto(
    Guid Id,
    Guid ProblemId,
    SolutionStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    ProfileSnapshotDto Author,
    List<SolutionContentBlockDto> ContentBlocks,
    int UpvotesCount,
    int DownvotesCount,
    int DiscussionsCount,
    VoteType? CurrentUserVote
);
