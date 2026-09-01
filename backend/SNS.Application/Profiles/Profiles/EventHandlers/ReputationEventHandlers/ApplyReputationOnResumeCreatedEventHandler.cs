using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.Profiles.Profiles.Constants;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Profiles.Profiles.Enums;
using SNS.Domain.Resumes.Events;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Application.Profiles.Profiles.EventHandlers.ReputationEventHandlers;

/// <summary>
/// Handles <see cref="ResumeCreatedIntegrationEvent"/> to award reputation points to the resume owner.
/// </summary>
public sealed class ApplyReputationOnResumeCreatedEventHandler
    : INotificationHandler<DomainEventNotification<ResumeCreatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IRepository<ReputationLedger> _ledgerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<ApplyReputationOnResumeCreatedEventHandler> _logger;

    public ApplyReputationOnResumeCreatedEventHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Profile> profileRepo,
        IRepository<ReputationLedger> ledgerRepo,
        IUnitOfWork unitOfWork,
        IAppLogger<ApplyReputationOnResumeCreatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _profileRepo = profileRepo;
        _ledgerRepo = ledgerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<ResumeCreatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var alreadyProcessed = await _dbContext.ReputationLedgers
            .AnyAsync(l => l.ProfileId == domainEvent.ProfileId &&
                           l.ActionType == ReputationActionType.CreatedResume &&
                           l.SourceEntityId == domainEvent.ResumeId,
                      cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning("Reputation for ResumeCreated already applied for Profile {ProfileId}, Resume {ResumeId}",
                domainEvent.ProfileId, domainEvent.ResumeId);
            return;
        }

        var profile = await _profileRepo.GetByIdAsync(domainEvent.ProfileId, cancellationToken);
        if (profile == null)
        {
            _logger.LogWarning("Profile not found when applying reputation for ResumeCreated: {ProfileId}", domainEvent.ProfileId);
            return;
        }

        profile.AdjustReputation(ReputationPointValues.ResumeCreated);

        var ledgerEntry = ReputationLedger.Create(
            profileId: domainEvent.ProfileId,
            actionType: ReputationActionType.CreatedResume,
            pointsDelta: ReputationPointValues.ResumeCreated,
            sourceEntityId: domainEvent.ResumeId);

        _ledgerRepo.Add(ledgerEntry);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
