using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Posts.Events;
using SNS.Domain.Profiles.Profiles.Constants;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Profiles.Profiles.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Application.Profiles.Profiles.EventHandlers.ReputationEventHandlers;

/// <summary>
/// Handles <see cref="PostDeletedIntegrationEvent"/> to reverse reputation points previously awarded for post creation.
/// </summary>
public sealed class ApplyReputationOnPostDeletedEventHandler
    : INotificationHandler<DomainEventNotification<PostDeletedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IRepository<ReputationLedger> _ledgerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<ApplyReputationOnPostDeletedEventHandler> _logger;

    public ApplyReputationOnPostDeletedEventHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Profile> profileRepo,
        IRepository<ReputationLedger> ledgerRepo,
        IUnitOfWork unitOfWork,
        IAppLogger<ApplyReputationOnPostDeletedEventHandler> logger)
    {
        _dbContext = dbContext;
        _profileRepo = profileRepo;
        _ledgerRepo = ledgerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<PostDeletedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var alreadyProcessed = await _dbContext.ReputationLedgers
            .AnyAsync(l => l.ProfileId == domainEvent.ProfileId &&
                           l.ActionType == ReputationActionType.PostDeleted &&
                           l.SourceEntityId == domainEvent.PostId,
                      cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning("Reputation penalty for PostDeleted already applied for Profile {ProfileId}, Post {PostId}",
                domainEvent.ProfileId, domainEvent.PostId);
            return;
        }

        var profile = await _profileRepo.GetByIdAsync(domainEvent.ProfileId, cancellationToken);
        if (profile == null)
        {
            _logger.LogWarning("Profile not found when applying penalty for PostDeleted: {ProfileId}", domainEvent.ProfileId);
            return;
        }

        profile.AdjustReputation(ReputationPointValues.PostDeleted);

        var ledgerEntry = ReputationLedger.Create(
            profileId: domainEvent.ProfileId,
            actionType: ReputationActionType.PostDeleted,
            pointsDelta: ReputationPointValues.PostDeleted,
            sourceEntityId: domainEvent.PostId);

        _ledgerRepo.Add(ledgerEntry);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
