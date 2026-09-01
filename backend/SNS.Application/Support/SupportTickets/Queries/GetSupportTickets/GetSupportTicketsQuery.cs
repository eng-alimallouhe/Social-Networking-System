using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;
using SNS.Application.Support.SupportTickets.Contracts;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Support.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Support;

namespace SNS.Application.Support.SupportTickets.Queries.GetSupportTickets;

public sealed record GetSupportTicketsQuery(
    int PageSize = 10,
    int CurrentPage = 1,
    TicketStatus? Status = null,
    TicketPriority? Priority = null,
    SupportTeckitCategory? Category = null,
    Guid? AssignedAgentId = null
) : IQuery<Paged<SupportTicketSummaryDto>>;

internal sealed class GetSupportTicketsQueryHandler : IQueryHandler<GetSupportTicketsQuery, Paged<SupportTicketSummaryDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;
    private readonly IApplicationDbContext _dbContext;

    public GetSupportTicketsQueryHandler(
        ICurrentUserService currentUserService,
        IPermissionService permissionService,
        IApplicationDbContext dbContext)
    {
        _currentUserService = currentUserService;
        _permissionService = permissionService;
        _dbContext = dbContext;
    }

    public async Task<Result<Paged<SupportTicketSummaryDto>>> Handle(GetSupportTicketsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.RoleType))
        {
            return Result<Paged<SupportTicketSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var hasViewPermission = await _permissionService.HasPermissionAsync(
            _currentUserService.RoleType,
            Permissions.Support.TicketsView,
            cancellationToken);

        if (!hasViewPermission)
        {
            return Result<Paged<SupportTicketSummaryDto>>.Failure(SupportStatusCodes.UnauthorizedAccess);
        }

        var query = _dbContext.SupportTickets.AsNoTracking();

        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
        }

        if (request.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == request.Priority.Value);
        }

        if (request.Category.HasValue)
        {
            query = query.Where(t => t.Category == request.Category.Value);
        }

        if (request.AssignedAgentId.HasValue)
        {
            query = query.Where(t => t.AssignedAgentId == request.AssignedAgentId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.UpdatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new SupportTicketSummaryDto(
                t.Id,
                t.UserId,
                t.AssignedAgentId,
                t.Title,
                t.Category,
                t.Priority,
                t.Status,
                t.Messages.Count,
                t.CreatedAt,
                t.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        return Result<Paged<SupportTicketSummaryDto>>.Success(new Paged<SupportTicketSummaryDto>(
            items: items,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage
        ), OperationStatusCode.Success);
    }
}
