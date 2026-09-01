using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.Problems.Commands.DeleteProblem;

/// <summary>
/// Command to soft-delete an existing discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem to delete.</param>
public sealed record DeleteProblemCommand(Guid ProblemId) : ICommand;

/// <summary>
/// Handles <see cref="DeleteProblemCommand"/> to verify ownership and mark the problem as inactive.
/// </summary>
internal sealed class DeleteProblemCommandHandler : ICommandHandler<DeleteProblemCommand>
{
    private readonly ISoftDeletableRepository<Problem> _problemRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProblemCommandHandler(
        ISoftDeletableRepository<Problem> problemRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _problemRepo = problemRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteProblemCommand request, CancellationToken cancellationToken)
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

        problem.SoftDelete();
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProblemStatusCodes.ProblemDeleted);
    }
}
