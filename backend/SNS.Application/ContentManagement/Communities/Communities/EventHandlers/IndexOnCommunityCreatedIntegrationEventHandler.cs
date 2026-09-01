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
/// Handles <see cref="CommunityCreatedIntegrationEvent"/> to synchronize the newly created community with the search index.
/// </summary>
public sealed class IndexOnCommunityCreatedIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<CommunityCreatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICommunitySearchService _communitySearchService;
    private readonly IAppLogger<IndexOnCommunityCreatedIntegrationEventHandler> _logger;

    public IndexOnCommunityCreatedIntegrationEventHandler(
        IApplicationDbContext dbContext,
        ICommunitySearchService communitySearchService,
        IAppLogger<IndexOnCommunityCreatedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _communitySearchService = communitySearchService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<CommunityCreatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var communityId = notification.DomainEvent.CommunityId;

        var communityDocument = await _dbContext.Communities
            .AsNoTracking()
            .Where(c => c.Id == communityId)
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
            _logger.LogWarning("Community not found for search indexing: {CommunityId}", communityId);
            return;
        }

        var result = await _communitySearchService.UpsertCommunityAsync(communityDocument, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to index community in search: {CommunityId}", communityId);
        }
    }
}
