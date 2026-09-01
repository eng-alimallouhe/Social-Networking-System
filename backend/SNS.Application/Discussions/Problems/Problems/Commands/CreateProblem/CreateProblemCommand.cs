using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Discussions.Problems.Events;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.Problems.Commands.CreateProblem;

/// <summary>
/// Command to create a new discussion problem with title, difficulty level, optional community, content blocks, and tags.
/// </summary>
/// <param name="Title">The problem title.</param>
/// <param name="Level">The difficulty level.</param>
/// <param name="CommunityId">Optional community ID if posted within a community.</param>
/// <param name="ContentBlocks">The ordered structured content blocks.</param>
/// <param name="Tags">Optional tag names to associate with the problem.</param>
public sealed record CreateProblemCommand(
    string Title,
    DifficultyLevel Level,
    Guid? CommunityId,
    List<CreateProblemContentBlockDto> ContentBlocks,
    List<string>? Tags
) : ICommand<Guid>;

/// <summary>
/// Handles <see cref="CreateProblemCommand"/> to validate input, persist the problem and content blocks, and trigger the classification event pipeline.
/// </summary>
internal sealed class CreateProblemCommandHandler : ICommandHandler<CreateProblemCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Problem> _problemRepo;
    private readonly IRepository<ProblemContentBlock> _contentBlockRepo;
    private readonly IRepository<ProblemTag> _problemTagRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateProblemCommandHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Problem> problemRepo,
        IRepository<ProblemContentBlock> contentBlockRepo,
        IRepository<ProblemTag> problemTagRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _problemRepo = problemRepo;
        _contentBlockRepo = contentBlockRepo;
        _problemTagRepo = problemTagRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateProblemCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result<Guid>.Failure(OperationStatusCode.InvalidInput);
        }

        if (request.CommunityId.HasValue)
        {
            var communityExists = await _dbContext.Communities
                .AnyAsync(c => c.Id == request.CommunityId.Value && c.IsActive, cancellationToken);

            if (!communityExists)
            {
                return Result<Guid>.Failure(ResourceStatusCode.NotFound);
            }
        }

        var problem = Problem.Create(
            authorId: profileId.Value,
            communityId: request.CommunityId,
            title: request.Title.Trim(),
            level: request.Level);

        _problemRepo.Add(problem);

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

        if (request.Tags != null && request.Tags.Any())
        {
            var normalizedTagNames = request.Tags
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim().ToLower())
                .Distinct()
                .ToList();

            if (normalizedTagNames.Any())
            {
                var existingTags = await _dbContext.Tags
                    .Where(t => normalizedTagNames.Contains(t.Name.ToLower()))
                    .ToListAsync(cancellationToken);

                var problemTags = existingTags
                    .Select(tag => ProblemTag.Create(problem.Id, tag.Id))
                    .ToList();

                if (problemTags.Any())
                {
                    _problemTagRepo.AddRange(problemTags);
                }
            }
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(
            new DomainEventNotification<ProblemCreatedEvent>(
                new ProblemCreatedEvent(problem.Id, DateTime.UtcNow)),
            cancellationToken);

        return Result<Guid>.Success(problem.Id, ProblemStatusCodes.ProblemCreated);
    }
}
