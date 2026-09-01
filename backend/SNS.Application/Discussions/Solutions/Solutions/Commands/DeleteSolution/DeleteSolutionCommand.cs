using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Solutions.Solutions.Commands.DeleteSolution;

/// <summary>
/// Command to soft-delete an existing solution.
/// </summary>
/// <param name="SolutionId">The unique identifier of the solution to delete.</param>
public sealed record DeleteSolutionCommand(Guid SolutionId) : ICommand;

/// <summary>
/// Handles <see cref="DeleteSolutionCommand"/> to verify ownership and mark the solution as inactive.
/// </summary>
internal sealed class DeleteSolutionCommandHandler : ICommandHandler<DeleteSolutionCommand>
{
    private readonly ISoftDeletableRepository<Solution> _solutionRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSolutionCommandHandler(
        ISoftDeletableRepository<Solution> solutionRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _solutionRepo = solutionRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSolutionCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var solution = await _solutionRepo.GetByIdAsync(request.SolutionId, cancellationToken);
        if (solution == null || !solution.IsActive)
        {
            return Result.Failure(SolutionStatusCodes.SolutionNotFound);
        }

        if (solution.AuthorId != profileId.Value)
        {
            return Result.Failure(SolutionStatusCodes.NotSolutionOwner);
        }

        solution.SoftDelete();
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(SolutionStatusCodes.SolutionDeleted);
    }
}
