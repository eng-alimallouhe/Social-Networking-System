using MediatR;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Search.ContentManagement.Communitites.Abstractions;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Communities.Events;

namespace SNS.Application.ContentManagement.Communities.Communities.EventHandlers;

/// <summary>
/// Handles <see cref="CommunityDeletedIntegrationEvent"/> to remove the community from the search index.
/// </summary>
public sealed class IndexOnCommunityDeletedIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<CommunityDeletedIntegrationEvent>>
{
    private readonly ICommunitySearchService _communitySearchService;
    private readonly IAppLogger<IndexOnCommunityDeletedIntegrationEventHandler> _logger;

    public IndexOnCommunityDeletedIntegrationEventHandler(
        ICommunitySearchService communitySearchService,
        IAppLogger<IndexOnCommunityDeletedIntegrationEventHandler> logger)
    {
        _communitySearchService = communitySearchService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<CommunityDeletedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var communityId = notification.DomainEvent.CommunityId;

        var result = await _communitySearchService.DeleteCommunityAsync(communityId.ToString(), cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to delete community from search index: {CommunityId}", communityId);
        }
    }
}
