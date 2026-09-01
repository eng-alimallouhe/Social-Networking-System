using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Support.Entities;
using SNS.Domain.Support.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Support;

namespace SNS.Application.Support.SupportTickets.Commands.CreateSupportTicket;

public sealed record CreateSupportTicketCommand(
    string Title,
    SupportTeckitCategory Category,
    TicketPriority Priority,
    string InitialMessage,
    IReadOnlyCollection<string>? AttachmentObjectKeys = null
) : ICommand<Guid>;

internal sealed class CreateSupportTicketCommandHandler : ICommandHandler<CreateSupportTicketCommand, Guid>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<SupportTicket> _supportTicketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSupportTicketCommandHandler(
        ICurrentUserService currentUserService,
        IRepository<SupportTicket> supportTicketRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _supportTicketRepository = supportTicketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateSupportTicketCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var ticket = SupportTicket.Create(
            userId: currentUserId.Value,
            title: request.Title,
            category: request.Category,
            priority: request.Priority,
            initialMessage: request.InitialMessage,
            attachmentObjectKeys: request.AttachmentObjectKeys);

        _supportTicketRepository.Add(ticket);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(ticket.Id, SupportStatusCodes.TicketCreated);
    }
}
