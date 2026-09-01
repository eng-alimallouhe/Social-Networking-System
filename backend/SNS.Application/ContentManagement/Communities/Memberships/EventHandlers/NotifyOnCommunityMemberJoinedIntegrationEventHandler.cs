using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.ContentManagement.Communities.Events;

namespace SNS.Application.ContentManagement.Communities.Memberships.EventHandlers;

/// <summary>
/// Handles <see cref="CommunityMemberJoinedIntegrationEvent"/> to notify community admins/moderators when a member joins.
/// </summary>
public sealed class NotifyOnCommunityMemberJoinedIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<CommunityMemberJoinedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IAppLogger<NotifyOnCommunityMemberJoinedIntegrationEventHandler> _logger;

    public NotifyOnCommunityMemberJoinedIntegrationEventHandler(
        IApplicationDbContext dbContext,
        IAppLogger<NotifyOnCommunityMemberJoinedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<CommunityMemberJoinedIntegrationEvent> notification, CancellationToken cancellationToken)
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
            _logger.LogWarning("Community not found for member joined notification: {CommunityId}", domainEvent.CommunityId);
            return;
        }

        var member = await _dbContext.Profiles
            .AsNoTracking()
            .Where(p => p.Id == domainEvent.MemberId && p.IsActive)
            .Select(p => new { p.Id, p.FullName })
            .FirstOrDefaultAsync(cancellationToken);

        if (member == null)
        {
            _logger.LogWarning("Member profile not found: {MemberId}", domainEvent.MemberId);
            return;
        }

        var recipientIds = community.AdminIds
            .Append(community.OwnerId)
            .Distinct()
            .Where(id => id != domainEvent.MemberId)
            .ToList();

        _logger.LogInformation(
            "Member {MemberName} ({MemberId}) joined community {CommunityName} ({CommunityId}). Notifying {Count} admins.",
            member.FullName,
            member.Id,
            community.Name,
            community.Id,
            recipientIds.Count);
    }
}
