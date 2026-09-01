using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Discussions.Solutions.Solutions.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Solutions.Solutions.Commands.CreateSolution;

/// <summary>
/// Command to submit a new proposed solution for an open discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem to solve.</param>
/// <param name="ContentBlocks">The structured content blocks comprising the solution.</param>
public sealed record CreateSolutionCommand(
    Guid ProblemId,
    List<CreateSolutionContentBlockDto> ContentBlocks
) : ICommand<Guid>;

/// <summary>
/// Handles <see cref="CreateSolutionCommand"/> to validate problem status and persist the solution.
/// </summary>
internal sealed class CreateSolutionCommandHandler : ICommandHandler<CreateSolutionCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Solution> _solutionRepo;
    private readonly IRepository<SolutionContentBlock> _contentBlockRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSolutionCommandHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Solution> solutionRepo,
        IRepository<SolutionContentBlock> contentBlockRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _solutionRepo = solutionRepo;
        _contentBlockRepo = contentBlockRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateSolutionCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var problem = await _dbContext.Problems
            .FirstOrDefaultAsync(p => p.Id == request.ProblemId && p.IsActive, cancellationToken);

        if (problem == null)
        {
            return Result<Guid>.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        if (problem.Status == ProblemStatus.Closed)
        {
            return Result<Guid>.Failure(ProblemStatusCodes.ProblemClosed);
        }

        if (request.ContentBlocks == null || !request.ContentBlocks.Any())
        {
            return Result<Guid>.Failure(OperationStatusCode.InvalidInput);
        }

        var solution = Solution.Create(request.ProblemId, profileId.Value);
        _solutionRepo.Add(solution);

        var blocks = request.ContentBlocks
            .OrderBy(b => b.Order)
            .Select(b => SolutionContentBlock.Create(
                solutionId: solution.Id,
                type: b.Type,
                content: b.Content,
                extraInfo: b.ExtraInfo,
                order: b.Order))
            .ToList();

        _contentBlockRepo.AddRange(blocks);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(solution.Id, SolutionStatusCodes.SolutionCreated);
    }
}
