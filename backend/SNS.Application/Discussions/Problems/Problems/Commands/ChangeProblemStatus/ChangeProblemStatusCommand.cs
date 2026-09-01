using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.Problems.Commands.ChangeProblemStatus;

/// <summary>
/// Command to update the lifecycle status of a discussion problem (Open, Solved, Closed).
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
/// <param name="Status">The new problem status.</param>
public sealed record ChangeProblemStatusCommand(
    Guid ProblemId,
    ProblemStatus Status
) : ICommand;

/// <summary>
/// Handles <see cref="ChangeProblemStatusCommand"/> to verify ownership and update problem status.
/// </summary>
internal sealed class ChangeProblemStatusCommandHandler : ICommandHandler<ChangeProblemStatusCommand>
{
    private readonly ISoftDeletableRepository<Problem> _problemRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeProblemStatusCommandHandler(
        ISoftDeletableRepository<Problem> problemRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _problemRepo = problemRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeProblemStatusCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var problem = await _problemRepo.GetByIdAsync(request.ProblemId, cancellationToken);
        if (problem == null || !problem.IsActive)
        {
            return Result.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        if (problem.AuthorId != profileId.Value)
        {
            return Result.Failure(ProblemStatusCodes.NotProblemOwner);
        }

        problem.ChangeStatus(request.Status);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProblemStatusCodes.ProblemStatusChanged);
    }
}
