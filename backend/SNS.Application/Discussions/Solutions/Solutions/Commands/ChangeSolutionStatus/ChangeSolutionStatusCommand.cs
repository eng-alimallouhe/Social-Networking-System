using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Domain.Discussions.Solutions.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Solutions.Solutions.Commands.ChangeSolutionStatus;

/// <summary>
/// Command to update the status of a proposed solution (e.g., Accepted, BestSolution, Rejected).
/// </summary>
/// <param name="SolutionId">The unique identifier of the solution.</param>
/// <param name="Status">The new status to apply.</param>
public sealed record ChangeSolutionStatusCommand(
    Guid SolutionId,
    SolutionStatus Status
) : ICommand;

/// <summary>
/// Handles <see cref="ChangeSolutionStatusCommand"/> to verify problem/solution ownership and update solution status.
/// </summary>
internal sealed class ChangeSolutionStatusCommandHandler : ICommandHandler<ChangeSolutionStatusCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Solution> _solutionRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeSolutionStatusCommandHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Solution> solutionRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _solutionRepo = solutionRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeSolutionStatusCommand request, CancellationToken cancellationToken)
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

        var problem = await _dbContext.Problems
            .FirstOrDefaultAsync(p => p.Id == solution.ProblemId && p.IsActive, cancellationToken);

        if (problem == null)
        {
            return Result.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        // Only problem owner or solution author can update status
        if (problem.AuthorId != profileId.Value && solution.AuthorId != profileId.Value)
        {
            return Result.Failure(SolutionStatusCodes.NotSolutionOwner);
        }

        solution.ChangeStatus(request.Status);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(SolutionStatusCodes.SolutionStatusChanged);
    }
}
