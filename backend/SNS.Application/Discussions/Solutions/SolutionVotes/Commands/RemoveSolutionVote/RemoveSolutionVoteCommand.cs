using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Solutions.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Solutions.SolutionVotes.Commands.RemoveSolutionVote;

/// <summary>
/// Command to remove an existing vote from a proposed solution.
/// </summary>
/// <param name="SolutionId">The unique identifier of the solution.</param>
public sealed record RemoveSolutionVoteCommand(Guid SolutionId) : ICommand;

/// <summary>
/// Handles <see cref="RemoveSolutionVoteCommand"/> to delete an active solution vote.
/// </summary>
internal sealed class RemoveSolutionVoteCommandHandler : ICommandHandler<RemoveSolutionVoteCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<SolutionVote> _voteRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveSolutionVoteCommandHandler(
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

    public async Task<Result> Handle(RemoveSolutionVoteCommand request, CancellationToken cancellationToken)
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

        if (existingVote == null)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        _voteRepo.Delete(existingVote);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(SolutionStatusCodes.VoteRemoved);
    }
}
