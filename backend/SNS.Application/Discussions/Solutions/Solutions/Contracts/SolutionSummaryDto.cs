using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.Discussions.Solutions.Enums;

namespace SNS.Application.Discussions.Solutions.Solutions.Contracts;

/// <summary>
/// Represents summary information for a proposed solution in list views and feeds.
/// </summary>
/// <param name="Id">The unique identifier of the solution.</param>
/// <param name="ProblemId">The associated problem identifier.</param>
/// <param name="Status">The solution status.</param>
/// <param name="Author">Snapshot of the solution author profile.</param>
/// <param name="UpvotesCount">The count of upvotes.</param>
/// <param name="DownvotesCount">The count of downvotes.</param>
/// <param name="DiscussionsCount">The count of threaded discussion comments.</param>
/// <param name="CreatedAt">The timestamp when the solution was submitted.</param>
/// <param name="UpdatedAt">The timestamp of the last update.</param>
public sealed record SolutionSummaryDto(
    Guid Id,
    Guid ProblemId,
    SolutionStatus Status,
    ProfileSnapshotDto Author,
    int UpvotesCount,
    int DownvotesCount,
    int DiscussionsCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
