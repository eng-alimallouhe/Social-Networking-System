using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Search.ContentManagement.Communitites.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Communities.Events;
using SNS.Domain.Search.Documents;

namespace SNS.Application.ContentManagement.Communities.Communities.EventHandlers;

/// <summary>
/// Handles <see cref="CommunityUpdatedIntegrationEvent"/> to update the community in the search index.
/// </summary>
public sealed class IndexOnCommunityUpdatedIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<CommunityUpdatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICommunitySearchService _communitySearchService;
    private readonly IAppLogger<IndexOnCommunityUpdatedIntegrationEventHandler> _logger;

    public IndexOnCommunityUpdatedIntegrationEventHandler(
        IApplicationDbContext dbContext,
        ICommunitySearchService communitySearchService,
        IAppLogger<IndexOnCommunityUpdatedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _communitySearchService = communitySearchService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<CommunityUpdatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var communityId = notification.DomainEvent.CommunityId;

        var communityDocument = await _dbContext.Communities
            .AsNoTracking()
            .Where(c => c.Id == communityId && c.IsActive)
            .Select(c => new CommunityDocument
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Type = c.Type,
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (communityDocument == null)
        {
            _logger.LogWarning("Community not found for search update: {CommunityId}", communityId);
            return;
        }

        var result = await _communitySearchService.UpsertCommunityAsync(communityDocument, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update community in search index: {CommunityId}", communityId);
        }
    }
}
