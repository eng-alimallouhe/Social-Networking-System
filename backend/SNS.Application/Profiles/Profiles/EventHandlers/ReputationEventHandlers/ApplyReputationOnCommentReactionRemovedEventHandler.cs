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
/// Handles <see cref="CommentReactionRemovedIntegrationEvent"/> to reverse reputation points when a reaction is removed from a comment.
/// </summary>
public sealed class ApplyReputationOnCommentReactionRemovedEventHandler
    : INotificationHandler<DomainEventNotification<CommentReactionRemovedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IRepository<ReputationLedger> _ledgerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<ApplyReputationOnCommentReactionRemovedEventHandler> _logger;

    public ApplyReputationOnCommentReactionRemovedEventHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Profile> profileRepo,
        IRepository<ReputationLedger> ledgerRepo,
        IUnitOfWork unitOfWork,
        IAppLogger<ApplyReputationOnCommentReactionRemovedEventHandler> logger)
    {
        _dbContext = dbContext;
        _profileRepo = profileRepo;
        _ledgerRepo = ledgerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<CommentReactionRemovedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var alreadyProcessed = await _dbContext.ReputationLedgers
            .AnyAsync(l => l.ProfileId == domainEvent.AuthorProfileId &&
                           l.ActionType == ReputationActionType.CommentReactionRemoved &&
                           l.SourceEntityId == domainEvent.ReactionId,
                      cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning("Reputation deduction for CommentReactionRemoved already applied for Reaction {ReactionId}", domainEvent.ReactionId);
            return;
        }

        var profile = await _profileRepo.GetByIdAsync(domainEvent.AuthorProfileId, cancellationToken);
        if (profile == null)
        {
            _logger.LogWarning("Profile not found when reversing reputation for CommentReactionRemoved: {ProfileId}", domainEvent.AuthorProfileId);
            return;
        }

        profile.AdjustReputation(ReputationPointValues.CommentReactionRemoved);

        var ledgerEntry = ReputationLedger.Create(
            profileId: domainEvent.AuthorProfileId,
            actionType: ReputationActionType.CommentReactionRemoved,
            pointsDelta: ReputationPointValues.CommentReactionRemoved,
            sourceEntityId: domainEvent.ReactionId);

        _ledgerRepo.Add(ledgerEntry);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
