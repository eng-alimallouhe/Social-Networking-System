using MediatR;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Application.Identity.Users.UsersManagement.EventHandlers.UserActivatedEvent;

public class ActivateProfileOnUserActivatedIntegrationEventHandler :
    INotificationHandler<DomainEventNotification<UserActivatedSynchronousEvent>>
{
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<ActivateProfileOnUserActivatedIntegrationEventHandler> _logger;
    public ActivateProfileOnUserActivatedIntegrationEventHandler(
        ISoftDeletableRepository<Profile> profileRepo,
        IUnitOfWork unitOfWork,
        IAppLogger<ActivateProfileOnUserActivatedIntegrationEventHandler> logger)
    {
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<UserActivatedSynchronousEvent> notification, CancellationToken cancellationToken)
    {
        var profile = await _profileRepo.GetSingleByExpressionAsync(
            p => p.UserId == notification.DomainEvent.UserId, cancellationToken);

        if (profile == null)
        {
            _logger.LogWarning("Profile not found for user {UserId}", notification.DomainEvent.UserId);
            return;
        }

        try
        {
            profile.Activate();
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while trying to activate user profile with name: {FullName}, and UserId: {UserId}", ex, profile.FullName, profile.UserId);
        }
    }
}
