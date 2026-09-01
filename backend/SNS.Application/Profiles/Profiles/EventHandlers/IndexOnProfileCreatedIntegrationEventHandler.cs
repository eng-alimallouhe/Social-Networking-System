using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.Profiles.Profiles.Events;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Profiles.Profiles.EventHandlers;

/// <summary>
/// Handles <see cref="ProfileCreatedIntegrationEvent"/> to synchronize the new profile with the search index.
/// </summary>
public class IndexOnProfileCreatedIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<ProfileCreatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IProfileSearchService _profileSearchService;
    private readonly IAppLogger<IndexOnProfileCreatedIntegrationEventHandler> _logger;

    public IndexOnProfileCreatedIntegrationEventHandler(
        IApplicationDbContext dbContext,
        IProfileSearchService profileSearchService,
        IAppLogger<IndexOnProfileCreatedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _profileSearchService = profileSearchService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<ProfileCreatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var profileId = notification.DomainEvent.ProfileId;

        var profileDocument = await _dbContext.Profiles
            .AsNoTracking()
            .Where(p => p.Id == profileId)
            .Select(p => new ProfileDocument
            {
                Id = p.Id,
                FullName = p.FullName,
                Specialization = p.Specialization,
                Bio = p.Bio,
                Universities = p.AcademicRecords.Select(ar => ar.University.Name).ToList(),
                CreatedAt = p.CreatedAt,
                Skills = p.ProfileSkills.Select(ps => ps.Skill.Name).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (profileDocument == null)
        {
            _logger.LogWarning("Profile not found for search indexing: {ProfileId}", profileId);
            return;
        }

        var result = await _profileSearchService.UpsertProfileAsync(profileDocument, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to index profile in search: {ProfileId}", profileId);
        }
    }
}
