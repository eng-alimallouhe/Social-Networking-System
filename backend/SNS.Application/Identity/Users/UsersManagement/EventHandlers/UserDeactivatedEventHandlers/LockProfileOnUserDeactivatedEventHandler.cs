using MediatR;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Exceptions;

namespace SNS.Application.Identity.Users.UsersManagement.EventHandlers.UserDeactivatedEventHandlers;

public class LockProfileOnUserDeactivatedEventHandler :
    INotificationHandler<DomainEventNotification<UserDeactivatedEvent>>
{
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<LockProfileOnUserDeactivatedEventHandler> _logger;

    public LockProfileOnUserDeactivatedEventHandler(
        ISoftDeletableRepository<Profile> profileRepo, IUnitOfWork unitOfWork, IAppLogger<LockProfileOnUserDeactivatedEventHandler> logger)
    {
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<UserDeactivatedEvent> notification, CancellationToken cancellationToken)
    {
        var profile = await _profileRepo.GetSingleByExpressionAsync(
            p => p.UserId == notification.DomainEvent.UserId, cancellationToken);

        if (profile == null)
        {
            _logger.LogError(
                "Can't Find the profile for user with Id: {UserId}",
                new EntityNotFoundException(""),
                notification.DomainEvent.UserId);
            return;
        }
        try {
            profile.SoftDelete();

            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while deactivation the profile for user: {UserId}", ex, notification.DomainEvent.UserId);
            throw;
        }
    }
}
