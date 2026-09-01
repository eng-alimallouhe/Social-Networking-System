using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.ProblemVotes.Commands.RemoveProblemVote;

/// <summary>
/// Command to remove an existing vote from a discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
public sealed record RemoveProblemVoteCommand(Guid ProblemId) : ICommand;

/// <summary>
/// Handles <see cref="RemoveProblemVoteCommand"/> to delete an active vote.
/// </summary>
internal sealed class RemoveProblemVoteCommandHandler : ICommandHandler<RemoveProblemVoteCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<ProblemVote> _voteRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProblemVoteCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<ProblemVote> voteRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _voteRepo = voteRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveProblemVoteCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var problemExists = await _dbContext.Problems
            .AnyAsync(p => p.Id == request.ProblemId && p.IsActive, cancellationToken);

        if (!problemExists)
        {
            return Result.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        var existingVote = await _voteRepo.GetSingleByExpressionAsync(
            v => v.ProblemId == request.ProblemId && v.VoterId == profileId.Value,
            cancellationToken);

        if (existingVote == null)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        _voteRepo.Delete(existingVote);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProblemStatusCodes.VoteRemoved);
    }
}
