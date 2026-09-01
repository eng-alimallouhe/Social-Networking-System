using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.ContentManagement.Communities.Events;

namespace SNS.Application.ContentManagement.Communities.Memberships.EventHandlers;

/// <summary>
/// Handles <see cref="CommunityMembershipRequestedIntegrationEvent"/> to notify community admins/moderators about new join requests.
/// </summary>
public sealed class NotifyOnCommunityMembershipRequestedIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<CommunityMembershipRequestedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IAppLogger<NotifyOnCommunityMembershipRequestedIntegrationEventHandler> _logger;

    public NotifyOnCommunityMembershipRequestedIntegrationEventHandler(
        IApplicationDbContext dbContext,
        IAppLogger<NotifyOnCommunityMembershipRequestedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<CommunityMembershipRequestedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var community = await _dbContext.Communities
            .AsNoTracking()
            .Where(c => c.Id == domainEvent.CommunityId && c.IsActive)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.OwnerId,
                AdminIds = c.Memberships
                    .Where(m => (m.Role == CommunityRole.Moderator || m.Role == CommunityRole.Owner) && m.Status == CommunityMembershipStatus.Active)
                    .Select(m => m.MemberId)
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (community == null)
        {
            _logger.LogWarning("Community not found for membership request notification: {CommunityId}", domainEvent.CommunityId);
            return;
        }

        var submitter = await _dbContext.Profiles
            .AsNoTracking()
            .Where(p => p.Id == domainEvent.SubmitterId && p.IsActive)
            .Select(p => new { p.Id, p.FullName })
            .FirstOrDefaultAsync(cancellationToken);

        if (submitter == null)
        {
            _logger.LogWarning("Submitter profile not found for membership request: {ProfileId}", domainEvent.SubmitterId);
            return;
        }

        var recipientIds = community.AdminIds
            .Append(community.OwnerId)
            .Distinct()
            .Where(id => id != domainEvent.SubmitterId)
            .ToList();

        _logger.LogInformation(
            "Profile {SubmitterName} ({SubmitterId}) requested to join private community {CommunityName} ({CommunityId}). Notifying {Count} admins.",
            submitter.FullName,
            submitter.Id,
            community.Name,
            community.Id,
            recipientIds.Count);
    }
}
