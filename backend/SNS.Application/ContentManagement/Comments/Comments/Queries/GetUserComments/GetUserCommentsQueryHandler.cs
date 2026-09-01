using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Comments.Comments.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Comments.Comments.Queries.GetUserComments;

internal sealed class GetUserCommentsQueryHandler : IQueryHandler<GetUserCommentsQuery, Paged<CommentSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetUserCommentsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<CommentSummaryDto>>> Handle(GetUserCommentsQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        var targetProfileId = request.ProfileId ?? currentProfileId;

        if (!targetProfileId.HasValue)
        {
            return Result<Paged<CommentSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        if (pageSize > 50) pageSize = 50;

        var baseQuery = _dbContext.Comments
            .AsNoTracking()
            .Where(c => c.AuthorId == targetProfileId.Value && c.IsActive);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var rawComments = await baseQuery
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.PostId,
                c.ParentCommentId,
                c.Content,
                c.CreatedAt,
                c.UpdatedAt,
                AuthorId = c.AuthorId,
                AuthorFullName = c.Author.FullName,
                AuthorSpecialization = c.Author.Specialization,
                AuthorProfilePictureKey = c.Author.ProfilePictureObjectKey,
                ReactionsCount = c.Reactions.Count(),
                RepliesCount = c.Replies.Count(r => r.IsActive),
                CurrentUserReaction = currentProfileId != null ? c.Reactions.Where(r => r.ReactorId == currentProfileId).Select(r => (ReactionType?)r.Type).FirstOrDefault() : null
            })
            .ToListAsync(cancellationToken);

        var distinctKeys = rawComments
            .Select(c => c.AuthorProfilePictureKey)
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

        var items = rawComments.Select(c => new CommentSummaryDto(
            c.Id,
            c.PostId,
            c.ParentCommentId,
            c.Content,
            c.CreatedAt,
            c.UpdatedAt,
            new ProfileSnapshotDto(
                c.AuthorId,
                c.AuthorFullName,
                c.AuthorSpecialization,
                c.AuthorProfilePictureKey != null && urlMap.TryGetValue(c.AuthorProfilePictureKey, out var picUrl) ? picUrl : null
            ),
            c.ReactionsCount,
            c.RepliesCount,
            c.CurrentUserReaction
        )).ToList();

        var pagedResult = new Paged<CommentSummaryDto>(
            items,
            totalCount,
            pageSize,
            page
        );

        return Result<Paged<CommentSummaryDto>>.Success(pagedResult, OperationStatusCode.Success);
    }
}
