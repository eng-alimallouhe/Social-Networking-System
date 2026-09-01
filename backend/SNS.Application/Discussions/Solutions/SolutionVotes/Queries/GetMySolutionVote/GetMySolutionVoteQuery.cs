using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Solutions.SolutionVotes.Queries.GetMySolutionVote;

/// <summary>
/// Query to retrieve the current authenticated user's vote on a specific solution.
/// </summary>
/// <param name="SolutionId">The unique identifier of the solution.</param>
public sealed record GetMySolutionVoteQuery(Guid SolutionId) : IQuery<VoteType?>;

/// <summary>
/// Handles <see cref="GetMySolutionVoteQuery"/> to return the user's vote type.
/// </summary>
internal sealed class GetMySolutionVoteQueryHandler : IQueryHandler<GetMySolutionVoteQuery, VoteType?>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetMySolutionVoteQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<VoteType?>> Handle(GetMySolutionVoteQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<VoteType?>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var solutionExists = await _dbContext.Solutions
            .AnyAsync(s => s.Id == request.SolutionId && s.IsActive, cancellationToken);

        if (!solutionExists)
        {
            return Result<VoteType?>.Failure(SolutionStatusCodes.SolutionNotFound);
        }

        var voteType = await _dbContext.SolutionVotes
            .AsNoTracking()
            .Where(v => v.SolutionId == request.SolutionId && v.VoterId == profileId.Value)
            .Select(v => (VoteType?)v.Type)
            .FirstOrDefaultAsync(cancellationToken);

        return Result<VoteType?>.Success(voteType, OperationStatusCode.Success);
    }
}
