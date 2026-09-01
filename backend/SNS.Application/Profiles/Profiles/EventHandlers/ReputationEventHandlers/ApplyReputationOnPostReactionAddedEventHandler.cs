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
/// Handles <see cref="PostReactionAddedIntegrationEvent"/> to award reputation points to the post author when reacted to.
/// </summary>
public sealed class ApplyReputationOnPostReactionAddedEventHandler
    : INotificationHandler<DomainEventNotification<PostReactionAddedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IRepository<ReputationLedger> _ledgerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<ApplyReputationOnPostReactionAddedEventHandler> _logger;

    public ApplyReputationOnPostReactionAddedEventHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Profile> profileRepo,
        IRepository<ReputationLedger> ledgerRepo,
        IUnitOfWork unitOfWork,
        IAppLogger<ApplyReputationOnPostReactionAddedEventHandler> logger)
    {
        _dbContext = dbContext;
        _profileRepo = profileRepo;
        _ledgerRepo = ledgerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<PostReactionAddedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var alreadyProcessed = await _dbContext.ReputationLedgers
            .AnyAsync(l => l.ProfileId == domainEvent.AuthorProfileId &&
                           l.ActionType == ReputationActionType.PostReactionAdded &&
                           l.SourceEntityId == domainEvent.ReactionId,
                      cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning("Reputation for PostReactionAdded already applied for Reaction {ReactionId}", domainEvent.ReactionId);
            return;
        }

        var profile = await _profileRepo.GetByIdAsync(domainEvent.AuthorProfileId, cancellationToken);
        if (profile == null)
        {
            _logger.LogWarning("Profile not found when applying reputation for PostReactionAdded: {ProfileId}", domainEvent.AuthorProfileId);
            return;
        }

        profile.AdjustReputation(ReputationPointValues.PostReactionAdded);

        var ledgerEntry = ReputationLedger.Create(
            profileId: domainEvent.AuthorProfileId,
            actionType: ReputationActionType.PostReactionAdded,
            pointsDelta: ReputationPointValues.PostReactionAdded,
            sourceEntityId: domainEvent.ReactionId);

        _ledgerRepo.Add(ledgerEntry);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
