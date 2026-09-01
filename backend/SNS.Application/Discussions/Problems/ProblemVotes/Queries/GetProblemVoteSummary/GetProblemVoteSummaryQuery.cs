using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Problems.ProblemVotes.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;

namespace SNS.Application.Discussions.Problems.ProblemVotes.Queries.GetProblemVoteSummary;

/// <summary>
/// Query to retrieve aggregate voting metrics and current user vote status for a problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
public sealed record GetProblemVoteSummaryQuery(Guid ProblemId) : IQuery<ProblemVoteSummaryDto>;

/// <summary>
/// Handles <see cref="GetProblemVoteSummaryQuery"/> to calculate vote metrics.
/// </summary>
internal sealed class GetProblemVoteSummaryQueryHandler : IQueryHandler<GetProblemVoteSummaryQuery, ProblemVoteSummaryDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetProblemVoteSummaryQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ProblemVoteSummaryDto>> Handle(GetProblemVoteSummaryQuery request, CancellationToken cancellationToken)
    {
        var problemExists = await _dbContext.Problems
            .AnyAsync(p => p.Id == request.ProblemId && p.IsActive, cancellationToken);

        if (!problemExists)
        {
            return Result<ProblemVoteSummaryDto>.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        var currentProfileId = _currentUserService.ProfileId;

        var votes = await _dbContext.ProblemVotes
            .AsNoTracking()
            .Where(v => v.ProblemId == request.ProblemId)
            .Select(v => new { v.VoterId, v.Type })
            .ToListAsync(cancellationToken);

        var upvotesCount = votes.Count(v => v.Type == VoteType.Upvote);
        var downvotesCount = votes.Count(v => v.Type == VoteType.Downvote);
        var totalScore = upvotesCount - downvotesCount;

        var currentUserVote = currentProfileId.HasValue
            ? votes.Where(v => v.VoterId == currentProfileId.Value).Select(v => (VoteType?)v.Type).FirstOrDefault()
            : null;

        return Result<ProblemVoteSummaryDto>.Success(new ProblemVoteSummaryDto(
            ProblemId: request.ProblemId,
            UpvotesCount: upvotesCount,
            DownvotesCount: downvotesCount,
            TotalScore: totalScore,
            CurrentUserVote: currentUserVote), OperationStatusCode.Success);
    }
}
