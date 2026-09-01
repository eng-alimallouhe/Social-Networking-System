using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Resumes.Enums;
using SNS.Domain.Resumes.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Profiles;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Resumes.Commands.CreateResume;

/// <summary>
/// Represents a command to create a new resume with title, summary, visual template, and language preferences.
/// </summary>
/// <param name="PersonalPictureUrl">The optional personal picture storage object key if not syncing with profile.</param>
/// <param name="SyncProfilePicture">Whether the resume synchronizes its picture with the profile avatar.</param>
/// <param name="Title">The title or professional designation for the resume.</param>
/// <param name="Template">The visual layout template.</param>
/// <param name="Summary">The professional executive summary.</param>
/// <param name="Language">The supported language for the resume.</param>
public sealed record CreateResumeCommand(
    string? PersonalPictureUrl,
    bool SyncProfilePicture,
    string Title,
    Template Template,
    string Summary,
    SupportedLanguage Language
) : ICommand<Guid>;

/// <summary>
/// Handles the execution of <see cref="CreateResumeCommand"/> to create and persist a new resume.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Validates user reputation tier and active resume creation limit.
/// 3. Validates basic title and summary inputs.
/// 4. Instantiates the <see cref="Resume"/> aggregate root with the specified configuration.
/// 5. Persists the resume to the database via unit of work.
/// 6. Publishes <see cref="ResumeCreatedIntegrationEvent"/> for event-driven reputation update.
/// Side effects include database insert, reputation ledger insert, and transaction commit.
/// </remarks>
internal sealed class CreateResumeCommandHandler : ICommandHandler<CreateResumeCommand, Guid>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _dbContext;
    private readonly IReputationPolicyService _reputationPolicyService;
    private readonly IMediator _mediator;

    public CreateResumeCommandHandler(
        ISoftDeletableRepository<Resume> resumeRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IApplicationDbContext dbContext,
        IReputationPolicyService reputationPolicyService,
        IMediator mediator)
    {
        _resumeRepo = resumeRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _reputationPolicyService = reputationPolicyService;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateResumeCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var profile = await _dbContext.Profiles
            .FirstOrDefaultAsync(p => p.Id == profileId.Value && p.IsActive, cancellationToken);

        if (profile == null)
        {
            return Result<Guid>.Failure(ProfileStatusCodes.NotFound);
        }

        var currentResumeCount = await _dbContext.Resumes
            .CountAsync(r => r.OwnerId == profileId.Value && r.IsActive, cancellationToken);

        if (!_reputationPolicyService.CanCreateCV(profile.Reputation, currentResumeCount))
        {
            return Result<Guid>.Failure(ProfileStatusCodes.MaxResumeLimitReached);
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result<Guid>.Failure(OperationStatusCode.InvalidInput);
        }

        var resume = Resume.Create(
            ownerId: profileId.Value,
            personalPictureUrl: request.PersonalPictureUrl,
            syncProfilePicture: request.SyncProfilePicture,
            title: request.Title,
            template: request.Template,
            summary: request.Summary ?? string.Empty,
            langauge: request.Language
        );

        _resumeRepo.Add(resume);
        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(
            new DomainEventNotification<ResumeCreatedIntegrationEvent>(
                new ResumeCreatedIntegrationEvent(profileId.Value, resume.Id, DateTime.UtcNow)),
            cancellationToken);

        return Result<Guid>.Success(resume.Id, ResumeStatusCodes.ResumeCreated);
    }
}
