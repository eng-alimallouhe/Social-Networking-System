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
/// Handles <see cref="PostReactionRemovedIntegrationEvent"/> to reverse reputation points when a reaction is removed from a post.
/// </summary>
public sealed class ApplyReputationOnPostReactionRemovedEventHandler
    : INotificationHandler<DomainEventNotification<PostReactionRemovedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IRepository<ReputationLedger> _ledgerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<ApplyReputationOnPostReactionRemovedEventHandler> _logger;

    public ApplyReputationOnPostReactionRemovedEventHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Profile> profileRepo,
        IRepository<ReputationLedger> ledgerRepo,
        IUnitOfWork unitOfWork,
        IAppLogger<ApplyReputationOnPostReactionRemovedEventHandler> logger)
    {
        _dbContext = dbContext;
        _profileRepo = profileRepo;
        _ledgerRepo = ledgerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<PostReactionRemovedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var alreadyProcessed = await _dbContext.ReputationLedgers
            .AnyAsync(l => l.ProfileId == domainEvent.AuthorProfileId &&
                           l.ActionType == ReputationActionType.PostReactionRemoved &&
                           l.SourceEntityId == domainEvent.ReactionId,
                      cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning("Reputation deduction for PostReactionRemoved already applied for Reaction {ReactionId}", domainEvent.ReactionId);
            return;
        }

        var profile = await _profileRepo.GetByIdAsync(domainEvent.AuthorProfileId, cancellationToken);
        if (profile == null)
        {
            _logger.LogWarning("Profile not found when reversing reputation for PostReactionRemoved: {ProfileId}", domainEvent.AuthorProfileId);
            return;
        }

        profile.AdjustReputation(ReputationPointValues.PostReactionRemoved);

        var ledgerEntry = ReputationLedger.Create(
            profileId: domainEvent.AuthorProfileId,
            actionType: ReputationActionType.PostReactionRemoved,
            pointsDelta: ReputationPointValues.PostReactionRemoved,
            sourceEntityId: domainEvent.ReactionId);

        _ledgerRepo.Add(ledgerEntry);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
