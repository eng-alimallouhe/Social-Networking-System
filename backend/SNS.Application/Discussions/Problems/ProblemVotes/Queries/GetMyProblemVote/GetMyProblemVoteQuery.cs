using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.ProblemVotes.Queries.GetMyProblemVote;

/// <summary>
/// Query to retrieve the current authenticated user's vote on a specific discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
public sealed record GetMyProblemVoteQuery(Guid ProblemId) : IQuery<VoteType?>;

/// <summary>
/// Handles <see cref="GetMyProblemVoteQuery"/> to return the user's vote type.
/// </summary>
internal sealed class GetMyProblemVoteQueryHandler : IQueryHandler<GetMyProblemVoteQuery, VoteType?>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetMyProblemVoteQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<VoteType?>> Handle(GetMyProblemVoteQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<VoteType?>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var problemExists = await _dbContext.Problems
            .AnyAsync(p => p.Id == request.ProblemId && p.IsActive, cancellationToken);

        if (!problemExists)
        {
            return Result<VoteType?>.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        var voteType = await _dbContext.ProblemVotes
            .AsNoTracking()
            .Where(v => v.ProblemId == request.ProblemId && v.VoterId == profileId.Value)
            .Select(v => (VoteType?)v.Type)
            .FirstOrDefaultAsync(cancellationToken);

        return Result<VoteType?>.Success(voteType, OperationStatusCode.Success);
    }
}
