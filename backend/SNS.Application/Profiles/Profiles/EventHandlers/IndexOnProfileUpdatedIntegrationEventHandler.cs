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
/// Handles <see cref="ProfileUpdatedIntegrationEvent"/> to synchronize updated profile searchable properties with the search index.
/// </summary>
public class IndexOnProfileUpdatedIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<ProfileUpdatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IProfileSearchService _profileSearchService;
    private readonly IAppLogger<IndexOnProfileUpdatedIntegrationEventHandler> _logger;

    public IndexOnProfileUpdatedIntegrationEventHandler(
        IApplicationDbContext dbContext,
        IProfileSearchService profileSearchService,
        IAppLogger<IndexOnProfileUpdatedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _profileSearchService = profileSearchService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<ProfileUpdatedIntegrationEvent> notification, CancellationToken cancellationToken)
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
            _logger.LogWarning("Profile not found for search index update: {ProfileId}", profileId);
            return;
        }

        var result = await _profileSearchService.UpsertProfileAsync(profileDocument, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update profile in search index: {ProfileId}", profileId);
        }
    }
}
