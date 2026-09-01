using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Support.Entities;
using SNS.Domain.Support.Enums;
using SNS.Shared.Exceptions;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Support;

namespace SNS.Application.Support.TicketMessages.Commands.ReplyToSupportTicket;

public sealed record ReplyToSupportTicketCommand(
    Guid TicketId,
    string MessageBody,
    IReadOnlyCollection<string>? AttachmentObjectKeys = null
) : ICommand;

internal sealed class ReplyToSupportTicketCommandHandler : ICommandHandler<ReplyToSupportTicketCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;
    private readonly IRepository<SupportTicket> _supportTicketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReplyToSupportTicketCommandHandler(
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

    public async Task<Result> Handle(ReplyToSupportTicketCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue || string.IsNullOrWhiteSpace(_currentUserService.RoleType))
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var ticket = await _supportTicketRepository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket == null)
        {
            return Result.Failure(SupportStatusCodes.TicketNotFound);
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            return Result.Failure(SupportStatusCodes.TicketClosed);
        }

        var isOwner = ticket.UserId == currentUserId.Value;

        if (isOwner)
        {
            try
            {
                ticket.AddUserReply(request.MessageBody, request.AttachmentObjectKeys);
            }
            catch (DomainException)
            {
                return Result.Failure(SupportStatusCodes.TicketClosed);
            }
        }
        else
        {
            var hasReplyPermission = await _permissionService.HasPermissionAsync(
                _currentUserService.RoleType,
                Permissions.Support.TicketsReply,
                cancellationToken);

            if (!hasReplyPermission)
            {
                return Result.Failure(SupportStatusCodes.UnauthorizedAccess);
            }

            try
            {
                ticket.AddAgentReply(currentUserId.Value, request.MessageBody, request.AttachmentObjectKeys);
            }
            catch (DomainException)
            {
                return Result.Failure(SupportStatusCodes.TicketClosed);
            }
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(SupportStatusCodes.ReplyAdded);
    }
}
