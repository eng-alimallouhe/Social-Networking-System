using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Application.Discussions.Solutions.SolutionVotes.Contracts;

/// <summary>
/// Represents aggregate voting metrics and current user vote status for a proposed solution.
/// </summary>
/// <param name="SolutionId">The unique identifier of the solution.</param>
/// <param name="UpvotesCount">The count of upvotes.</param>
/// <param name="DownvotesCount">The count of downvotes.</param>
/// <param name="TotalScore">Net vote score (Upvotes minus Downvotes).</param>
/// <param name="CurrentUserVote">The vote type cast by the authenticated user, if any.</param>
public sealed record SolutionVoteSummaryDto(
    Guid SolutionId,
    int UpvotesCount,
    int DownvotesCount,
    int TotalScore,
    VoteType? CurrentUserVote
);
