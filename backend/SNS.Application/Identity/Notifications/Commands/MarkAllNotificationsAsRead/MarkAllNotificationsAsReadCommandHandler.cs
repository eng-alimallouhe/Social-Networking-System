using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.Notifications.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
namespace SNS.Application.Identity.Notifications.Commands.MarkAllNotificationsAsRead;

public sealed class MarkAllNotificationsAsReadCommandHandler
    : ICommandHandler<MarkAllNotificationsAsReadCommand>
{
    private readonly IRepository<Notification> _notificationRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public MarkAllNotificationsAsReadCommandHandler(
        IRepository<Notification> notificationRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _notificationRepo = notificationRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId == null || currentUserId == Guid.Empty)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var unreadNotifications = await _notificationRepo
            .GetListByExpressionAsync(n => n.UserId == currentUserId && !n.IsRead, cancellationToken); ;

        if (!unreadNotifications.Any())
        {
            return Result.Success(OperationStatusCode.Success);
        }

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
