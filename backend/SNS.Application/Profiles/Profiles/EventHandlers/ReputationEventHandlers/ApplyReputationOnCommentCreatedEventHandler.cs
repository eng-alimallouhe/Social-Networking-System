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
/// Handles <see cref="CommentCreatedIntegrationEvent"/> to award reputation points to the comment author.
/// </summary>
public sealed class ApplyReputationOnCommentCreatedEventHandler
    : INotificationHandler<DomainEventNotification<CommentCreatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IRepository<ReputationLedger> _ledgerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<ApplyReputationOnCommentCreatedEventHandler> _logger;

    public ApplyReputationOnCommentCreatedEventHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Profile> profileRepo,
        IRepository<ReputationLedger> ledgerRepo,
        IUnitOfWork unitOfWork,
        IAppLogger<ApplyReputationOnCommentCreatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _profileRepo = profileRepo;
        _ledgerRepo = ledgerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<CommentCreatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var alreadyProcessed = await _dbContext.ReputationLedgers
            .AnyAsync(l => l.ProfileId == domainEvent.ProfileId &&
                           l.ActionType == ReputationActionType.CreatedComment &&
                           l.SourceEntityId == domainEvent.CommentId,
                      cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning("Reputation for CommentCreated already applied for Profile {ProfileId}, Comment {CommentId}",
                domainEvent.ProfileId, domainEvent.CommentId);
            return;
        }

        var profile = await _profileRepo.GetByIdAsync(domainEvent.ProfileId, cancellationToken);
        if (profile == null)
        {
            _logger.LogWarning("Profile not found when applying reputation for CommentCreated: {ProfileId}", domainEvent.ProfileId);
            return;
        }

        profile.AdjustReputation(ReputationPointValues.CommentCreated);

        var ledgerEntry = ReputationLedger.Create(
            profileId: domainEvent.ProfileId,
            actionType: ReputationActionType.CreatedComment,
            pointsDelta: ReputationPointValues.CommentCreated,
            sourceEntityId: domainEvent.CommentId);

        _ledgerRepo.Add(ledgerEntry);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
