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
/// Handles <see cref="ResumeDeletedIntegrationEvent"/> to reverse reputation points previously awarded for resume creation.
/// </summary>
public sealed class ApplyReputationOnResumeDeletedEventHandler
    : INotificationHandler<DomainEventNotification<ResumeDeletedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IRepository<ReputationLedger> _ledgerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<ApplyReputationOnResumeDeletedEventHandler> _logger;

    public ApplyReputationOnResumeDeletedEventHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Profile> profileRepo,
        IRepository<ReputationLedger> ledgerRepo,
        IUnitOfWork unitOfWork,
        IAppLogger<ApplyReputationOnResumeDeletedEventHandler> logger)
    {
        _dbContext = dbContext;
        _profileRepo = profileRepo;
        _ledgerRepo = ledgerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<ResumeDeletedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var alreadyProcessed = await _dbContext.ReputationLedgers
            .AnyAsync(l => l.ProfileId == domainEvent.ProfileId &&
                           l.ActionType == ReputationActionType.ResumeDeleted &&
                           l.SourceEntityId == domainEvent.ResumeId,
                      cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning("Reputation penalty for ResumeDeleted already applied for Profile {ProfileId}, Resume {ResumeId}",
                domainEvent.ProfileId, domainEvent.ResumeId);
            return;
        }

        var profile = await _profileRepo.GetByIdAsync(domainEvent.ProfileId, cancellationToken);
        if (profile == null)
        {
            _logger.LogWarning("Profile not found when applying penalty for ResumeDeleted: {ProfileId}", domainEvent.ProfileId);
            return;
        }

        profile.AdjustReputation(ReputationPointValues.ResumeDeleted);

        var ledgerEntry = ReputationLedger.Create(
            profileId: domainEvent.ProfileId,
            actionType: ReputationActionType.ResumeDeleted,
            pointsDelta: ReputationPointValues.ResumeDeleted,
            sourceEntityId: domainEvent.ResumeId);

        _ledgerRepo.Add(ledgerEntry);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
