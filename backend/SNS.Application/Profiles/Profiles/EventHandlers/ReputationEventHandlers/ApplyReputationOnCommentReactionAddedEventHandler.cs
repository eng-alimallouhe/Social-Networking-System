using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Comments.Events;
using SNS.Domain.Profiles.Profiles.Constants;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Profiles.Profiles.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Application.Profiles.Profiles.EventHandlers.ReputationEventHandlers;

/// <summary>
/// Handles <see cref="CommentReactionAddedIntegrationEvent"/> to award reputation points to the comment author when reacted to.
/// </summary>
public sealed class ApplyReputationOnCommentReactionAddedEventHandler
    : INotificationHandler<DomainEventNotification<CommentReactionAddedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IRepository<ReputationLedger> _ledgerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<ApplyReputationOnCommentReactionAddedEventHandler> _logger;

    public ApplyReputationOnCommentReactionAddedEventHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Profile> profileRepo,
        IRepository<ReputationLedger> ledgerRepo,
        IUnitOfWork unitOfWork,
        IAppLogger<ApplyReputationOnCommentReactionAddedEventHandler> logger)
    {
        _dbContext = dbContext;
        _profileRepo = profileRepo;
        _ledgerRepo = ledgerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<CommentReactionAddedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var alreadyProcessed = await _dbContext.ReputationLedgers
            .AnyAsync(l => l.ProfileId == domainEvent.AuthorProfileId &&
                           l.ActionType == ReputationActionType.CommentReactionAdded &&
                           l.SourceEntityId == domainEvent.ReactionId,
                      cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning("Reputation for CommentReactionAdded already applied for Reaction {ReactionId}", domainEvent.ReactionId);
            return;
        }

        var profile = await _profileRepo.GetByIdAsync(domainEvent.AuthorProfileId, cancellationToken);
        if (profile == null)
        {
            _logger.LogWarning("Profile not found when applying reputation for CommentReactionAdded: {ProfileId}", domainEvent.AuthorProfileId);
            return;
        }

        profile.AdjustReputation(ReputationPointValues.CommentReactionAdded);

        var ledgerEntry = ReputationLedger.Create(
            profileId: domainEvent.AuthorProfileId,
            actionType: ReputationActionType.CommentReactionAdded,
            pointsDelta: ReputationPointValues.CommentReactionAdded,
            sourceEntityId: domainEvent.ReactionId);

        _ledgerRepo.Add(ledgerEntry);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
