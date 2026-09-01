using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Search.Projects.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.Projects.Events;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Projects.EventHandlers;

/// <summary>
/// Handles <see cref="ProjectUpdatedIntegrationEvent"/> to synchronize updated project searchable details with the search index.
/// </summary>
public class IndexOnProjectUpdatedIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<ProjectUpdatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IProjectSearchService _projectSearchService;
    private readonly IAppLogger<IndexOnProjectUpdatedIntegrationEventHandler> _logger;

    public IndexOnProjectUpdatedIntegrationEventHandler(
        IApplicationDbContext dbContext,
        IProjectSearchService projectSearchService,
        IAppLogger<IndexOnProjectUpdatedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _projectSearchService = projectSearchService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<ProjectUpdatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var projectId = notification.DomainEvent.ProjectId;

        var projectDocument = await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.Id == projectId)
            .Select(p => new ProjectDocument
            {
                Id = p.Id,
                Title = p.Title,
                ShortDescription = p.ShortDescription,
                ReadmeContent = p.ReadmeContent,
                Type = p.Type,
                Status = p.Status,
                PublishedAt = p.PublishedAt,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Skills = p.Skills.Select(ps => ps.Skill.Name).ToList(),
                Tags = p.Tags.Select(pt => pt.Tag.Name).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (projectDocument == null)
        {
            _logger.LogWarning("Project not found for search index update: {ProjectId}", projectId);
            return;
        }

        var result = await _projectSearchService.UpsertProjectAsync(projectDocument, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update project in search index: {ProjectId}", projectId);
        }
    }
}
