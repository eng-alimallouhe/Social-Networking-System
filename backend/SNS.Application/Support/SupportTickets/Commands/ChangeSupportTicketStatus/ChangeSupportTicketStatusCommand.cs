using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Support.Entities;
using SNS.Domain.Support.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Support;

namespace SNS.Application.Support.SupportTickets.Commands.ChangeSupportTicketStatus;

public sealed record ChangeSupportTicketStatusCommand(
    Guid TicketId,
    TicketStatus Status
) : ICommand;

internal sealed class ChangeSupportTicketStatusCommandHandler : ICommandHandler<ChangeSupportTicketStatusCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;
    private readonly IRepository<SupportTicket> _supportTicketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeSupportTicketStatusCommandHandler(
        ICurrentUserService currentUserService,
        IPermissionService permissionService,
        IRepository<SupportTicket> supportTicketRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _permissionService = permissionService;
        _supportTicketRepository = supportTicketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeSupportTicketStatusCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.RoleType))
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var hasPermission = await _permissionService.HasPermissionAsync(
            _currentUserService.RoleType,
            Permissions.Support.TicketsChangeStatus,
            cancellationToken);

        if (!hasPermission)
        {
            return Result.Failure(SupportStatusCodes.UnauthorizedAccess);
        }

        var ticket = await _supportTicketRepository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket == null)
        {
            return Result.Failure(SupportStatusCodes.TicketNotFound);
        }

        ticket.ChangeStatus(request.Status);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(SupportStatusCodes.StatusChanged);
    }
}
