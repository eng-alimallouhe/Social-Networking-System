using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.ProblemVotes.Commands.AddOrChangeProblemVote;

/// <summary>
/// Command to cast or switch a vote (Upvote / Downvote) on a discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
/// <param name="Type">The vote type to apply.</param>
public sealed record AddOrChangeProblemVoteCommand(
    Guid ProblemId,
    VoteType Type
) : ICommand;

/// <summary>
/// Handles <see cref="AddOrChangeProblemVoteCommand"/> with idempotent voting behavior.
/// </summary>
internal sealed class AddOrChangeProblemVoteCommandHandler : ICommandHandler<AddOrChangeProblemVoteCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<ProblemVote> _voteRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrChangeProblemVoteCommandHandler(
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

    public async Task<Result> Handle(AddOrChangeProblemVoteCommand request, CancellationToken cancellationToken)
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

        // Case 1: No existing vote -> Create new vote
        if (existingVote == null)
        {
            var newVote = ProblemVote.Create(request.ProblemId, profileId.Value, request.Type);
            _voteRepo.Add(newVote);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success(ProblemStatusCodes.VoteAdded);
        }

        // Case 2: Existing vote has the same type -> Idempotent success (200 OK) without DB modification
        if (existingVote.Type == request.Type)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        // Case 3: Existing vote has different type -> Change vote type
        existingVote.ChangeVote(request.Type);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success(ProblemStatusCodes.VoteUpdated);
    }
}
