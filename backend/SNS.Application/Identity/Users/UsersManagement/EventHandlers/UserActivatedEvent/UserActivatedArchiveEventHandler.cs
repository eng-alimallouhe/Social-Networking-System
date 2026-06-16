using MediatR;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Archives;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Application.Identity.Users.UsersManagement.EventHandlers.UserActivatedEvent;

public class UserActivatedArchiveEventHandler :
    INotificationHandler<DomainEventNotification<UserActivatedIntegrationEvent>>
{
    private readonly IArchiveService _archiveService;
    private readonly IUnitOfWork _unitOfWork;

    public UserActivatedArchiveEventHandler(
        IArchiveService archiveService,
        IUnitOfWork unitOfWork)
    {
        _archiveService = archiveService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DomainEventNotification<UserActivatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        var archiveMessage = "User activated his account From DeviceName: " + notification.DomainEvent.Device + " and using this browser: " + notification.DomainEvent.Browser;


        var archiveRow = new CreateUserArchiveDto(
            UserId: notification.DomainEvent.UserId,
            ActionType: ActionType.AccountActivated, 
            PerformedBy: notification.DomainEvent.UserId, 
            Parameters: new Dictionary<ReplacementKey, string>
            {
                { ReplacementKey.Device,  domainEvent.Device},
                { ReplacementKey.Country,  domainEvent.Country},
                { ReplacementKey.City,  domainEvent.City},
            });

        await _archiveService.LogUserActionAsync(archiveRow, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
