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
/// Handles <see cref="CommentDeletedIntegrationEvent"/> to reverse reputation points previously awarded for comment creation.
/// </summary>
public sealed class ApplyReputationOnCommentDeletedEventHandler
    : INotificationHandler<DomainEventNotification<CommentDeletedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IRepository<ReputationLedger> _ledgerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<ApplyReputationOnCommentDeletedEventHandler> _logger;

    public ApplyReputationOnCommentDeletedEventHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Profile> profileRepo,
        IRepository<ReputationLedger> ledgerRepo,
        IUnitOfWork unitOfWork,
        IAppLogger<ApplyReputationOnCommentDeletedEventHandler> logger)
    {
        _dbContext = dbContext;
        _profileRepo = profileRepo;
        _ledgerRepo = ledgerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<CommentDeletedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var alreadyProcessed = await _dbContext.ReputationLedgers
            .AnyAsync(l => l.ProfileId == domainEvent.ProfileId &&
                           l.ActionType == ReputationActionType.CommentDeleted &&
                           l.SourceEntityId == domainEvent.CommentId,
                      cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning("Reputation penalty for CommentDeleted already applied for Profile {ProfileId}, Comment {CommentId}",
                domainEvent.ProfileId, domainEvent.CommentId);
            return;
        }

        var profile = await _profileRepo.GetByIdAsync(domainEvent.ProfileId, cancellationToken);
        if (profile == null)
        {
            _logger.LogWarning("Profile not found when applying penalty for CommentDeleted: {ProfileId}", domainEvent.ProfileId);
            return;
        }

        profile.AdjustReputation(ReputationPointValues.CommentDeleted);

        var ledgerEntry = ReputationLedger.Create(
            profileId: domainEvent.ProfileId,
            actionType: ReputationActionType.CommentDeleted,
            pointsDelta: ReputationPointValues.CommentDeleted,
            sourceEntityId: domainEvent.CommentId);

        _ledgerRepo.Add(ledgerEntry);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
