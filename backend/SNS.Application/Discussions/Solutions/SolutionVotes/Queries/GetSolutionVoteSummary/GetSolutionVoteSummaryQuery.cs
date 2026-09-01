using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Solutions.SolutionVotes.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;

namespace SNS.Application.Discussions.Solutions.SolutionVotes.Queries.GetSolutionVoteSummary;

/// <summary>
/// Query to retrieve aggregate voting metrics and current user vote status for a solution.
/// </summary>
/// <param name="SolutionId">The unique identifier of the solution.</param>
public sealed record GetSolutionVoteSummaryQuery(Guid SolutionId) : IQuery<SolutionVoteSummaryDto>;

/// <summary>
/// Handles <see cref="GetSolutionVoteSummaryQuery"/> to calculate vote metrics for a solution.
/// </summary>
internal sealed class GetSolutionVoteSummaryQueryHandler : IQueryHandler<GetSolutionVoteSummaryQuery, SolutionVoteSummaryDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetSolutionVoteSummaryQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<SolutionVoteSummaryDto>> Handle(GetSolutionVoteSummaryQuery request, CancellationToken cancellationToken)
    {
        var solutionExists = await _dbContext.Solutions
            .AnyAsync(s => s.Id == request.SolutionId && s.IsActive, cancellationToken);

        if (!solutionExists)
        {
            return Result<SolutionVoteSummaryDto>.Failure(SolutionStatusCodes.SolutionNotFound);
        }

        var currentProfileId = _currentUserService.ProfileId;

        var votes = await _dbContext.SolutionVotes
            .AsNoTracking()
            .Where(v => v.SolutionId == request.SolutionId)
            .Select(v => new { v.VoterId, v.Type })
            .ToListAsync(cancellationToken);

        var upvotesCount = votes.Count(v => v.Type == VoteType.Upvote);
        var downvotesCount = votes.Count(v => v.Type == VoteType.Downvote);
        var totalScore = upvotesCount - downvotesCount;

        var currentUserVote = currentProfileId.HasValue
            ? votes.Where(v => v.VoterId == currentProfileId.Value).Select(v => (VoteType?)v.Type).FirstOrDefault()
            : null;

        return Result<SolutionVoteSummaryDto>.Success(new SolutionVoteSummaryDto(
            SolutionId: request.SolutionId,
            UpvotesCount: upvotesCount,
            DownvotesCount: downvotesCount,
            TotalScore: totalScore,
            CurrentUserVote: currentUserVote), OperationStatusCode.Success);
    }
}
