using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Comments.CommentReactions.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.ContentManagement.Comments.CommentReactions.Queries.GetCommentReactions;

internal sealed class GetCommentReactionsQueryHandler : IQueryHandler<GetCommentReactionsQuery, Paged<CommentReactionSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetCommentReactionsQueryHandler(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<CommentReactionSummaryDto>>> Handle(GetCommentReactionsQuery request, CancellationToken cancellationToken)
    {
        var commentExists = await _dbContext.Comments
            .AnyAsync(c => c.Id == request.CommentId && c.IsActive, cancellationToken);

        if (!commentExists)
        {
            return Result<Paged<CommentReactionSummaryDto>>.Failure(ResourceStatusCode.NotFound);
        }

        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 20;
        if (pageSize > 100) pageSize = 100;

        var baseQuery = _dbContext.CommentReactions
            .AsNoTracking()
            .Where(cr => cr.CommentId == request.CommentId);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var rawReactions = await baseQuery
            .Join(
                _dbContext.Profiles.AsNoTracking(),
                cr => cr.ReactorId,
                p => p.Id,
                (cr, p) => new
                {
                    cr.ReactorId,
                    p.FullName,
                    p.Specialization,
                    p.ProfilePictureObjectKey,
                    cr.Type,
                    cr.CreatedAt
                })
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var distinctKeys = rawReactions
            .Select(r => r.ProfilePictureObjectKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct()
            .ToList();

        var urlTasks = distinctKeys.Select(async k => new
        {
            Key = k!,
            Url = await _fileStorageService.GetTemporaryUrlAsync(k!, TimeSpan.FromHours(1))
        });
        var resolvedUrls = await Task.WhenAll(urlTasks);
        var urlMap = resolvedUrls.ToDictionary(r => r.Key, r => r.Url);

        var reactions = rawReactions.Select(r => new CommentReactionSummaryDto(
            User: new ProfileSnapshotDto(
                r.ReactorId,
                r.FullName,
                r.Specialization,
                r.ProfilePictureObjectKey != null && urlMap.TryGetValue(r.ProfilePictureObjectKey, out var url) ? url : null
            ),
            ReactionType: (ReactionType)r.Type,
            ReactedAt: r.CreatedAt
        )).ToList();

        var pagedResult = new Paged<CommentReactionSummaryDto>(
            reactions,
            totalCount,
            pageSize,
            page
        );

        return Result<Paged<CommentReactionSummaryDto>>.Success(pagedResult, OperationStatusCode.Success);
    }
}
