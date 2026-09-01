using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Application.Discussions.Problems.ProblemVotes.Contracts;

/// <summary>
/// Represents aggregate voting metrics and current user vote status for a discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
/// <param name="UpvotesCount">The count of upvotes.</param>
/// <param name="DownvotesCount">The count of downvotes.</param>
/// <param name="TotalScore">Net vote score (Upvotes minus Downvotes).</param>
/// <param name="CurrentUserVote">The vote type cast by the authenticated user, if any.</param>
public sealed record ProblemVoteSummaryDto(
    Guid ProblemId,
    int UpvotesCount,
    int DownvotesCount,
    int TotalScore,
    VoteType? CurrentUserVote
);
