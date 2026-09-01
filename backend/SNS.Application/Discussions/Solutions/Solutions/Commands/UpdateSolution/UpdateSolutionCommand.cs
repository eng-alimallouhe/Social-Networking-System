using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Discussions.Solutions.Solutions.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Solutions.Solutions.Commands.UpdateSolution;

/// <summary>
/// Command to update the content blocks of an existing proposed solution.
/// </summary>
/// <param name="SolutionId">The unique identifier of the solution to update.</param>
/// <param name="ContentBlocks">The updated structured content blocks.</param>
public sealed record UpdateSolutionCommand(
    Guid SolutionId,
    List<CreateSolutionContentBlockDto> ContentBlocks
) : ICommand;

/// <summary>
/// Handles <see cref="UpdateSolutionCommand"/> to verify ownership and replace content blocks.
/// </summary>
internal sealed class UpdateSolutionCommandHandler : ICommandHandler<UpdateSolutionCommand>
{
    private readonly ISoftDeletableRepository<Solution> _solutionRepo;
    private readonly IRepository<SolutionContentBlock> _contentBlockRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSolutionCommandHandler(
        ISoftDeletableRepository<Solution> solutionRepo,
        IRepository<SolutionContentBlock> contentBlockRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _solutionRepo = solutionRepo;
        _contentBlockRepo = contentBlockRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateSolutionCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (request.ContentBlocks == null || !request.ContentBlocks.Any())
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
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

        await _contentBlockRepo.ExecuteDeleteAsync(
            cb => cb.SolutionId == solution.Id,
            cancellationToken);

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
        solution.Touch();

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(SolutionStatusCodes.SolutionUpdated);
    }
}
