using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Discussions.Problems.Events;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.Problems.Commands.UpdateProblem;

/// <summary>
/// Command to update an existing discussion problem's metadata and content blocks.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem to update.</param>
/// <param name="Title">The updated title.</param>
/// <param name="Level">The updated difficulty level.</param>
/// <param name="CommunityId">Optional updated community association.</param>
/// <param name="ContentBlocks">The updated collection of structured content blocks.</param>
public sealed record UpdateProblemCommand(
    Guid ProblemId,
    string Title,
    DifficultyLevel Level,
    Guid? CommunityId,
    List<CreateProblemContentBlockDto> ContentBlocks
) : ICommand;

/// <summary>
/// Handles <see cref="UpdateProblemCommand"/> to verify ownership, update problem details, refresh content blocks, and trigger reclassification.
/// </summary>
internal sealed class UpdateProblemCommandHandler : ICommandHandler<UpdateProblemCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Problem> _problemRepo;
    private readonly IRepository<ProblemContentBlock> _contentBlockRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateProblemCommandHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Problem> problemRepo,
        IRepository<ProblemContentBlock> contentBlockRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _problemRepo = problemRepo;
        _contentBlockRepo = contentBlockRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateProblemCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
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

        if (request.CommunityId.HasValue && request.CommunityId != problem.CommunityId)
        {
            var communityExists = await _dbContext.Communities
                .AnyAsync(c => c.Id == request.CommunityId.Value && c.IsActive, cancellationToken);

            if (!communityExists)
            {
                return Result.Failure(ResourceStatusCode.NotFound);
            }
        }

        problem.Update(request.Title.Trim(), request.Level, request.CommunityId);

        await _contentBlockRepo.ExecuteDeleteAsync(
            cb => cb.ProblemId == problem.Id,
            cancellationToken);

        if (request.ContentBlocks != null && request.ContentBlocks.Any())
        {
            var blocks = request.ContentBlocks
                .OrderBy(b => b.Order)
                .Select(b => ProblemContentBlock.Create(
                    problemId: problem.Id,
                    type: b.Type,
                    content: b.Content,
                    extraInfo: b.ExtraInfo,
                    order: b.Order))
                .ToList();

            _contentBlockRepo.AddRange(blocks);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(
            new DomainEventNotification<ProblemUpdatedEvent>(
                new ProblemUpdatedEvent(problem.Id, true, DateTime.UtcNow)),
            cancellationToken);

        return Result.Success(ProblemStatusCodes.ProblemUpdated);
    }
}
