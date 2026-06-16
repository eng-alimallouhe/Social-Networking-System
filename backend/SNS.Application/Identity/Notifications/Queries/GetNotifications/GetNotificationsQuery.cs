using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Identity.Notifications.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Notifications.Queries.GetNotifications;

public sealed record NotificationDto(
    Guid Id,
    Guid? ActorProfileId,
    NotificationSource Source,
    NotificationType Type,
    Guid TargetId,
    string RedirectUrl, 
    bool IsRead,
    DateTime CreatedAt);

public sealed record GetNotificationsQuery(
    bool? IsRead,
    int CurrentPage = 1,
    int PageSize = 10) : IQuery<Paged<NotificationDto>>;

public sealed class GetNotificationsQueryHandler
    : IQueryHandler<GetNotificationsQuery, Paged<NotificationDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetNotificationsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Paged<NotificationDto>>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        // 1️⃣ حارس الأمان: جلب هوية المستخدم الحالي من التوكن 🔒
        var currentUserId = _currentUserService.UserId;

        if (currentUserId == null || currentUserId == Guid.Empty)
        {
            return Result<Paged<NotificationDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        // 2️⃣ تجهيز الاستعلام الأساسي الموجه لجدول الإشعارات مع ميزة الـ AsNoTracking لسرعة خارقة
        // (تأكد من اسم جدول الإشعارات في الـ DbContext عندك، لنفترض أنه Notifications)
        var query = _dbContext.Notifications
            .Where(n => n.UserId == currentUserId);

        if (request.IsRead.HasValue)
        {
            query = query.Where(n => n.IsRead == request.IsRead.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationDto(
                n.Id,
                n.ActorProfileId,
                n.Source,
                n.Type,
                n.TargetId,
                n.RedirectUrl,
                n.IsRead,
                n.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        var hasNextPage = request.CurrentPage < totalPages;

        var paginatedResult = new Paged<NotificationDto>(
            items: items,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage);

        return Result<Paged<NotificationDto>>.Success(paginatedResult, OperationStatusCode.Success);
    }
}