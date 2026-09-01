using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Support.Entities;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Support;

namespace SNS.Application.Support.SupportTickets.Commands.AssignSupportTicket;

public sealed record AssignSupportTicketCommand(
    Guid TicketId,
    Guid AgentId
) : ICommand;

internal sealed class AssignSupportTicketCommandHandler : ICommandHandler<AssignSupportTicketCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<SupportTicket> _supportTicketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignSupportTicketCommandHandler(
        ICurrentUserService currentUserService,
        IPermissionService permissionService,
        IApplicationDbContext dbContext,
        IRepository<SupportTicket> supportTicketRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _permissionService = permissionService;
        _dbContext = dbContext;
        _supportTicketRepository = supportTicketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignSupportTicketCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.RoleType))
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var hasPermission = await _permissionService.HasPermissionAsync(
            _currentUserService.RoleType,
            Permissions.Support.TicketsAssign,
            cancellationToken);

        if (!hasPermission)
        {
            return Result.Failure(SupportStatusCodes.UnauthorizedAccess);
        }

        var agentExists = await _dbContext.Users
            .AnyAsync(u => u.Id == request.AgentId, cancellationToken);

        if (!agentExists)
        {
            return Result.Failure(SupportStatusCodes.InvalidAgent);
        }

        var ticket = await _supportTicketRepository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket == null)
        {
            return Result.Failure(SupportStatusCodes.TicketNotFound);
        }

        ticket.AssignToAgent(request.AgentId);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(SupportStatusCodes.TicketAssigned);
    }
}
