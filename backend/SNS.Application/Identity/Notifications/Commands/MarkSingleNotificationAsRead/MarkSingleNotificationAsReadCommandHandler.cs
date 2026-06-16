using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.Notifications.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Notifications.Commands.MarkSingleNotificationAsRead;


public sealed class MarkSingleNotificationAsReadCommandHandler
    : ICommandHandler<MarkSingleNotificationAsReadCommand>
{
    private readonly IRepository<Notification> _notificationRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public MarkSingleNotificationAsReadCommandHandler(
        IRepository<Notification> notificationRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _notificationRepo = notificationRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        MarkSingleNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId == null || currentUserId == Guid.Empty)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var notification = await _notificationRepo.GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification == null || notification.UserId != currentUserId)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (!notification.IsRead)
        {
            notification.MarkAsRead();
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}