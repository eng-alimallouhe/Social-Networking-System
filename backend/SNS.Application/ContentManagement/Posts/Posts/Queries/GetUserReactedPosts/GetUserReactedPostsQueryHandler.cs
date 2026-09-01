using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Posts.PostMentions.Contracts;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.ContentManagement.Posts.Posts.Queries.GetUserReactedPosts;

/// <summary>
/// Handles <see cref="GetUserReactedPostsQuery"/> to retrieve posts the current authenticated user has reacted to.
/// </summary>
internal sealed class GetUserReactedPostsQueryHandler : IQueryHandler<GetUserReactedPostsQuery, Paged<PostOverviewDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetUserReactedPostsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<PostOverviewDto>>> Handle(GetUserReactedPostsQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<Paged<PostOverviewDto>>.Failure(OperationStatusCode.AuthenticationRequired);
        }

        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        if (pageSize > 100) pageSize = 100;

        var baseQuery = _dbContext.PostReactions
            .AsNoTracking()
            .Where(pr => pr.ReactorId == profileId.Value)
            .Join(
                _dbContext.Posts.AsNoTracking().Where(p => p.IsActive),
                pr => pr.PostId,
                p => p.Id,
                (pr, p) => new { Reaction = pr, Post = p }
            );

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var rawPosts = await baseQuery
            .OrderByDescending(x => x.Reaction.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.Post.Id,
                AuthorId = x.Post.Author.Id,
                AuthorFullName = x.Post.Author.FullName,
                AuthorSpecialization = x.Post.Author.Specialization,
                AuthorProfilePictureKey = x.Post.Author.ProfilePictureObjectKey,

                x.Post.CommunityId,
                CommunityType = x.Post.Community != null ? x.Post.Community.Type : (SNS.Domain.ContentManagement.Communities.Enums.CommunityType?)null,
                CommunityName = x.Post.Community != null ? x.Post.Community.Name : null,
                CommunityLogoKey = x.Post.Community != null ? x.Post.Community.LogoObjectKey : null,

                x.Post.Title,
                x.Post.Content,
                x.Post.CreatedAt,
                x.Post.UpdatedAt,
                x.Post.LastInteractedAt,
                Media = x.Post.Media.OrderBy(m => m.Order).Select(m => new { m.ObjectKey, m.Order, m.Type }).ToList(),
                Tags = x.Post.PostTags.Select(pt => pt.Tag.Name).ToList(),
                CommentsCount = x.Post.Comments.Count(c => c.IsActive),
                ReactionsCount = x.Post.Reactions.Count,
                ViewsCount = x.Post.Views.Count,
                SavesCount = x.Post.SavedPosts.Count,
                ReactionType = (ReactionType?)x.Reaction.Type,

                Mentions = x.Post.Mentions
                    .Where(m => m.Profile.IsActive)
                    .Select(m => new
                    {
                        m.ProfileId,
                        DisplayName = m.Profile.FullName,
                        ProfilePictureKey = m.Profile.ProfilePictureObjectKey
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var distinctKeys = rawPosts
            .Select(p => p.AuthorProfilePictureKey)
            .Concat(rawPosts.Select(p => p.CommunityLogoKey))
            .Concat(rawPosts.SelectMany(p => p.Media.Select(m => m.ObjectKey)))
            .Concat(rawPosts.SelectMany(p => p.Mentions.Select(m => m.ProfilePictureKey)))
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

        var posts = rawPosts.Select(p => new PostOverviewDto(
            Id: p.Id,
            Author: new ProfileSnapshotDto(
                p.AuthorId,
                p.AuthorFullName,
                p.AuthorSpecialization,
                p.AuthorProfilePictureKey != null && urlMap.TryGetValue(p.AuthorProfilePictureKey, out var authorPicUrl) ? authorPicUrl : null
            ),
            Community: p.CommunityId.HasValue && p.CommunityType.HasValue && p.CommunityName != null
                ? new CommunitySnapshotDto(
                    p.CommunityId.Value,
                    p.CommunityName,
                    p.CommunityType.Value,
                    p.CommunityLogoKey != null && urlMap.TryGetValue(p.CommunityLogoKey, out var commLogoUrl) ? commLogoUrl : null)
                : null,
            Title: p.Title,
            Content: p.Content,
            CreatedAt: p.CreatedAt,
            UpdatedAt: p.UpdatedAt,
            LastInteractedAt: p.LastInteractedAt,
            Media: p.Media.Select(m => new PostMediaDto(
                Url: urlMap.TryGetValue(m.ObjectKey, out var mediaUrl) ? mediaUrl : m.ObjectKey,
                Order: m.Order,
                Type: m.Type
            )).ToList(),
            Tags: p.Tags,
            CommentsCount: p.CommentsCount,
            ReactionsCount: p.ReactionsCount,
            ViewsCount: p.ViewsCount,
            SavesCount: p.SavesCount,
            CurrentUserReaction: p.ReactionType,
            Mentions: p.Mentions.Select(m => new PostMentionDto(
                m.ProfileId,
                m.DisplayName,
                m.ProfilePictureKey != null && urlMap.TryGetValue(m.ProfilePictureKey, out var mentionPicUrl) ? mentionPicUrl : null
            )).ToList()
        )).ToList();

        var pagedResult = new Paged<PostOverviewDto>(
            posts,
            totalCount,
            pageSize,
            page
        );

        return Result<Paged<PostOverviewDto>>.Success(pagedResult, OperationStatusCode.Success);
    }
}
