using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;
using SNS.Application.Support.SupportTickets.Contracts;
using SNS.Domain.Support.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Support.SupportTickets.Queries.GetMySupportTickets;

public sealed record GetMySupportTicketsQuery(
    int PageSize = 10,
    int CurrentPage = 1,
    TicketStatus? Status = null
) : IQuery<Paged<SupportTicketSummaryDto>>;

internal sealed class GetMySupportTicketsQueryHandler : IQueryHandler<GetMySupportTicketsQuery, Paged<SupportTicketSummaryDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;

    public GetMySupportTicketsQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
    }

    public async Task<Result<Paged<SupportTicketSummaryDto>>> Handle(GetMySupportTicketsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue)
        {
            return Result<Paged<SupportTicketSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var query = _dbContext.SupportTickets
            .AsNoTracking()
            .Where(t => t.UserId == currentUserId.Value);

        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
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
