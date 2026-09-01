using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Support.TicketMessages.Contracts;
using SNS.Domain.Identity.Users.Constants;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Support;

namespace SNS.Application.Support.TicketMessages.Queries.GetTicketMessages;

public sealed record GetTicketMessagesQuery(Guid TicketId) : IQuery<IReadOnlyList<TicketMessageDto>>;

internal sealed class GetTicketMessagesQueryHandler : IQueryHandler<GetTicketMessagesQuery, IReadOnlyList<TicketMessageDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetTicketMessagesQueryHandler(
        ICurrentUserService currentUserService,
        IPermissionService permissionService,
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _currentUserService = currentUserService;
        _permissionService = permissionService;
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<IReadOnlyList<TicketMessageDto>>> Handle(GetTicketMessagesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue || string.IsNullOrWhiteSpace(_currentUserService.RoleType))
        {
            return Result<IReadOnlyList<TicketMessageDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var ticket = await _dbContext.SupportTickets
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

        if (ticket == null)
        {
            return Result<IReadOnlyList<TicketMessageDto>>.Failure(SupportStatusCodes.TicketNotFound);
        }

        var isOwner = ticket.UserId == currentUserId.Value;
        if (!isOwner)
        {
            var hasViewPermission = await _permissionService.HasPermissionAsync(
                _currentUserService.RoleType,
                Permissions.Support.TicketsView,
                cancellationToken);

            if (!hasViewPermission)
            {
                return Result<IReadOnlyList<TicketMessageDto>>.Failure(SupportStatusCodes.UnauthorizedAccess);
            }
        }

        var rawMessages = await _dbContext.TicketMessages
            .AsNoTracking()
            .Where(m => m.TicketId == request.TicketId)
            .Include(m => m.Attachments)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);

        var messages = rawMessages.Select(m => new TicketMessageDto(
            Id: m.Id,
            TicketId: m.TicketId,
            SenderId: m.SenderId,
            IsFromAgent: m.IsFromAgent,
            MessageBody: m.MessageBody,
            SentAt: m.SentAt,
            Attachments: m.Attachments.Select(a => new TicketAttachmentDto(
                Id: a.Id,
                ObjectKey: a.ObjectKey,
                PublicUrl: !string.IsNullOrWhiteSpace(a.ObjectKey) ? _fileStorageService.GetFilePublicUrl(a.ObjectKey) : null,
                CreatedAt: a.CreatedAt
            )).ToList()
        )).ToList();

        return Result<IReadOnlyList<TicketMessageDto>>.Success(messages, OperationStatusCode.Success);
    }
}
