using MediatR;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Application.Identity.Users.UsersManagement.EventHandlers.UserActivatedEvent;

public class UserActivatedProfileEventHandler :
    INotificationHandler<DomainEventNotification<UserActivatedSynchronousEvent>>
{
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<UserActivatedProfileEventHandler> _logger;
    public UserActivatedProfileEventHandler(
        ISoftDeletableRepository<Profile> profileRepo,
        IUnitOfWork unitOfWork,
        IAppLogger<UserActivatedProfileEventHandler> logger)
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

        profile.Activate();
    }
}
