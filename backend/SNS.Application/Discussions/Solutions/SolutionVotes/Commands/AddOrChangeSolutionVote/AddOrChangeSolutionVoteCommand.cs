using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Domain.Discussions.Solutions.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Solutions.SolutionVotes.Commands.AddOrChangeSolutionVote;

/// <summary>
/// Command to cast or switch a vote (Upvote / Downvote) on a proposed solution.
/// </summary>
/// <param name="SolutionId">The unique identifier of the solution.</param>
/// <param name="Type">The vote type to apply.</param>
public sealed record AddOrChangeSolutionVoteCommand(
    Guid SolutionId,
    VoteType Type
) : ICommand;

/// <summary>
/// Handles <see cref="AddOrChangeSolutionVoteCommand"/> with idempotent voting behavior.
/// </summary>
internal sealed class AddOrChangeSolutionVoteCommandHandler : ICommandHandler<AddOrChangeSolutionVoteCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<SolutionVote> _voteRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrChangeSolutionVoteCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<SolutionVote> voteRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _voteRepo = voteRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddOrChangeSolutionVoteCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var solutionExists = await _dbContext.Solutions
            .AnyAsync(s => s.Id == request.SolutionId && s.IsActive, cancellationToken);

        if (!solutionExists)
        {
            return Result.Failure(SolutionStatusCodes.SolutionNotFound);
        }

        var existingVote = await _voteRepo.GetSingleByExpressionAsync(
            v => v.SolutionId == request.SolutionId && v.VoterId == profileId.Value,
            cancellationToken);

        // Case 1: No existing vote -> Create new vote
        if (existingVote == null)
        {
            var newVote = SolutionVote.Create(profileId.Value, request.SolutionId, request.Type);
            _voteRepo.Add(newVote);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success(SolutionStatusCodes.VoteAdded);
        }

        // Case 2: Existing vote has the same type -> Idempotent success (200 OK) without DB modification
        if (existingVote.Type == request.Type)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        // Case 3: Existing vote has different type -> Change vote type
        existingVote.ChangeVote(request.Type);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success(SolutionStatusCodes.VoteUpdated);
    }
}
