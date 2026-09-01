using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.Discussions.Problems.Events;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Discussions.Problems.EventHandlers;

/// <summary>
/// Handles <see cref="ProblemCreatedIntegrationEvent"/> to synchronize the new discussion problem with the search index.
/// </summary>
public class IndexOnProblemCreatedIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<ProblemCreatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IProblemSearchService _problemSearchService;
    private readonly IAppLogger<IndexOnProblemCreatedIntegrationEventHandler> _logger;

    public IndexOnProblemCreatedIntegrationEventHandler(
        IApplicationDbContext dbContext,
        IProblemSearchService problemSearchService,
        IAppLogger<IndexOnProblemCreatedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _problemSearchService = problemSearchService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<ProblemCreatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var problemId = notification.DomainEvent.ProblemId;

        var problemDocument = await _dbContext.Problems
            .AsNoTracking()
            .Where(p => p.Id == problemId)
            .Select(p => new ProblemDocument
            {
                Id = p.Id,
                Title = p.Title,
                Status = p.Status,
                Level = p.Level,
                ContentBlocks = p.ContentBlocks
                    .OrderBy(cb => cb.Order)
                    .Select(cb => new ProblemBlockDocument
                    {
                        Type = cb.Type,
                        Content = cb.Content,
                        ExtraInfo = cb.ExtraInfo,
                        Order = cb.Order
                    })
                    .ToList(),
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Tags = p.ProblemTags.Select(pt => pt.Tag.Name).ToList(),
                Topics = p.ProblemTopics.Select(pt => pt.Topic.Name).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (problemDocument == null)
        {
            _logger.LogWarning("Problem not found for search indexing: {ProblemId}", problemId);
            return;
        }

        var result = await _problemSearchService.UpsertProblemAsync(problemDocument, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to index problem in search: {ProblemId}", problemId);
        }
    }
}
