using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Search.Identity.Users.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Identity.Users.UsersManagement.EventHandlers.UserIndexingEventHandlers;

/// <summary>
/// Handles <see cref="UserUpdatedIntegrationEvent"/> to synchronize updated user searchable properties with the search index.
/// </summary>
public class IndexOnUserUpdatedIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<UserUpdatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserSearchService _userSearchService;
    private readonly IAppLogger<IndexOnUserUpdatedIntegrationEventHandler> _logger;

    public IndexOnUserUpdatedIntegrationEventHandler(
        IApplicationDbContext dbContext,
        IUserSearchService userSearchService,
        IAppLogger<IndexOnUserUpdatedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _userSearchService = userSearchService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<UserUpdatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var userId = notification.DomainEvent.UserId;

        var userDocument = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserDocument
            {
                Id = u.Id,
                UserName = u.UserName,
                PreferredLanguage = u.PreferredLanguage,
                Role = u.Role.Type.ToString(),
                FullName = u.UserProfile != null ? u.UserProfile.FullName : null,
                Email = u.Email,
                Status = u.Status,
                DefaultCommunicationMethod = u.UserSecuritySettings.DefaultCommunicationMethod,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userDocument == null)
        {
            _logger.LogWarning("User not found for search index update: {UserId}", userId);
            return;
        }

        var result = await _userSearchService.UpsertUserAsync(userDocument, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update user in search index: {UserId}", userId);
        }
    }
}
