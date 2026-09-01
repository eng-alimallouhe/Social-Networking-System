using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Comments.Comments.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.ContentManagement.Comments.Comments.Queries.GetCommentById;

internal sealed class GetCommentByIdQueryHandler : IQueryHandler<GetCommentByIdQuery, CommentDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetCommentByIdQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<CommentDetailsDto>> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        var rawComment = await _dbContext.Comments
            .AsNoTracking()
            .Where(c => c.Id == request.CommentId && c.IsActive)
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
                CurrentUserReaction = profileId != null ? c.Reactions.Where(r => r.ReactorId == profileId).Select(r => (ReactionType?)r.Type).FirstOrDefault() : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (rawComment == null)
        {
            return Result<CommentDetailsDto>.Failure(ResourceStatusCode.NotFound);
        }

        // Direct parent comment
        var rawParent = rawComment.ParentCommentId.HasValue
            ? await _dbContext.Comments
                .AsNoTracking()
                .Where(c => c.Id == rawComment.ParentCommentId.Value && c.IsActive)
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
                    CurrentUserReaction = profileId != null ? c.Reactions.Where(r => r.ReactorId == profileId).Select(r => (ReactionType?)r.Type).FirstOrDefault() : null
                })
                .FirstOrDefaultAsync(cancellationToken)
            : null;

        var parentHasParent = rawParent != null && rawParent.ParentCommentId.HasValue;

        var distinctKeys = new List<string?>
        {
            rawComment.AuthorProfilePictureKey,
            rawParent?.AuthorProfilePictureKey
        }.Where(k => !string.IsNullOrWhiteSpace(k)).Distinct().ToList();

        var urlTasks = distinctKeys.Select(async k => new
        {
            Key = k!,
            Url = await _fileStorageService.GetTemporaryUrlAsync(k!, TimeSpan.FromHours(1))
        });
        var resolvedUrls = await Task.WhenAll(urlTasks);
        var urlMap = resolvedUrls.ToDictionary(r => r.Key, r => r.Url);

        var commentDto = new CommentSummaryDto(
            rawComment.Id,
            rawComment.PostId,
            rawComment.ParentCommentId,
            rawComment.Content,
            rawComment.CreatedAt,
            rawComment.UpdatedAt,
            new ProfileSnapshotDto(
                rawComment.AuthorId,
                rawComment.AuthorFullName,
                rawComment.AuthorSpecialization,
                rawComment.AuthorProfilePictureKey != null && urlMap.TryGetValue(rawComment.AuthorProfilePictureKey, out var cPicUrl) ? cPicUrl : null
            ),
            rawComment.ReactionsCount,
            rawComment.RepliesCount,
            rawComment.CurrentUserReaction
        );

        CommentSummaryDto? parentCommentDto = null;
        if (rawParent != null)
        {
            parentCommentDto = new CommentSummaryDto(
                rawParent.Id,
                rawParent.PostId,
                rawParent.ParentCommentId,
                rawParent.Content,
                rawParent.CreatedAt,
                rawParent.UpdatedAt,
                new ProfileSnapshotDto(
                    rawParent.AuthorId,
                    rawParent.AuthorFullName,
                    rawParent.AuthorSpecialization,
                    rawParent.AuthorProfilePictureKey != null && urlMap.TryGetValue(rawParent.AuthorProfilePictureKey, out var pPicUrl) ? pPicUrl : null
                ),
                rawParent.ReactionsCount,
                rawParent.RepliesCount,
                rawParent.CurrentUserReaction
            );
        }

        var details = new CommentDetailsDto(
            Comment: commentDto,
            ParentComment: parentCommentDto,
            ParentHasParent: parentHasParent
        );

        return Result<CommentDetailsDto>.Success(details, OperationStatusCode.Success);
    }
}
