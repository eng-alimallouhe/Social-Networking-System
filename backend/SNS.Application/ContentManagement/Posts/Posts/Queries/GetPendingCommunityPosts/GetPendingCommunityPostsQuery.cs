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
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.ContentManagement.Posts.Enums;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Posts.Posts.Queries.GetPendingCommunityPosts;

/// <summary>
/// Represents a query to retrieve paginated posts awaiting moderation approval in a community.
/// Only community owners and moderators can access this query.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="Page">The page index (1-based).</param>
/// <param name="PageSize">The page size.</param>
public sealed record GetPendingCommunityPostsQuery(
    Guid CommunityId,
    int Page = 1,
    int PageSize = 10
) : IQuery<Paged<PostOverviewDto>>;

/// <summary>
/// Handles the execution of <see cref="GetPendingCommunityPostsQuery"/> to retrieve posts awaiting approval.
/// </summary>
internal sealed class GetPendingCommunityPostsQueryHandler : IQueryHandler<GetPendingCommunityPostsQuery, Paged<PostOverviewDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetPendingCommunityPostsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<PostOverviewDto>>> Handle(GetPendingCommunityPostsQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<Paged<PostOverviewDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var community = await _dbContext.Communities
            .AsNoTracking()
            .Where(c => c.Id == request.CommunityId && c.IsActive)
            .Select(c => new { c.Id, c.OwnerId })
            .FirstOrDefaultAsync(cancellationToken);

        if (community == null)
        {
            return Result<Paged<PostOverviewDto>>.Failure(ResourceStatusCode.NotFound);
        }

        var isOwner = community.OwnerId == profileId.Value;
        var isModerator = !isOwner && await _dbContext.CommunityMemberships
            .AnyAsync(m => m.CommunityId == request.CommunityId &&
                           m.MemberId == profileId.Value &&
                           (m.Role == CommunityRole.Moderator || m.Role == CommunityRole.Owner) &&
                           m.Status == CommunityMembershipStatus.Active, cancellationToken);

        if (!isOwner && !isModerator)
        {
            return Result<Paged<PostOverviewDto>>.Failure(SecurityStatusCodes.UnAuthorized);
        }

        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        if (pageSize > 100) pageSize = 100;

        var baseQuery = _dbContext.Posts
            .AsNoTracking()
            .Where(p => p.CommunityId == request.CommunityId && p.IsActive && p.Status == PostStatus.Pending);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var rawPosts = await baseQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                AuthorId = p.Author.Id,
                AuthorFullName = p.Author.FullName,
                AuthorSpecialization = p.Author.Specialization,
                AuthorProfilePictureKey = p.Author.ProfilePictureObjectKey,

                p.CommunityId,
                CommunityType = p.Community != null ? p.Community.Type : (CommunityType?)null,
                CommunityName = p.Community != null ? p.Community.Name : null,
                CommunityLogoKey = p.Community != null ? p.Community.LogoObjectKey : null,

                p.Title,
                p.Content,
                p.CreatedAt,
                p.UpdatedAt,
                LastInteractedAt = p.Reactions.Max(r => (DateTime?)r.CreatedAt),

                Media = p.Media.OrderBy(m => m.Order).Select(m => new { m.ObjectKey, m.Order, m.Type }).ToList(),
                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
                CommentsCount = p.Comments.Count(c => c.IsActive),
                ReactionsCount = p.Reactions.Count,
                ViewsCount = p.Views.Count,
                SavesCount = p.SavedPosts.Count,
                CurrentUserReaction = p.Reactions.Where(r => r.ReactorId == profileId).Select(r => (ReactionType?)r.Type).FirstOrDefault(),

                Mentions = p.Mentions
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

        var posts = rawPosts.Select(p =>
        {
            var author = new ProfileSnapshotDto(
                p.AuthorId,
                p.AuthorFullName,
                p.AuthorSpecialization,
                !string.IsNullOrWhiteSpace(p.AuthorProfilePictureKey) && urlMap.TryGetValue(p.AuthorProfilePictureKey, out var authorPic) ? authorPic : null
            );

            var communityDto = p.CommunityId.HasValue ? new CommunitySnapshotDto(
                p.CommunityId.Value,
                p.CommunityName ?? string.Empty,
                p.CommunityType ?? CommunityType.Public,
                !string.IsNullOrWhiteSpace(p.CommunityLogoKey) && urlMap.TryGetValue(p.CommunityLogoKey, out var commLogo) ? commLogo : null
            ) : null;

            var mediaList = p.Media.Select(m => new PostMediaDto(
                !string.IsNullOrWhiteSpace(m.ObjectKey) && urlMap.TryGetValue(m.ObjectKey, out var mUrl) ? mUrl : string.Empty,
                m.Order,
                m.Type
            )).ToList();

            var mentionsList = p.Mentions.Select(m => new PostMentionDto(
                m.ProfileId,
                m.DisplayName,
                !string.IsNullOrWhiteSpace(m.ProfilePictureKey) && urlMap.TryGetValue(m.ProfilePictureKey, out var pic) ? pic : null
            )).ToList();

            return new PostOverviewDto(
                Id: p.Id,
                Author: author,
                Community: communityDto,
                Title: p.Title,
                Content: p.Content,
                CreatedAt: p.CreatedAt,
                UpdatedAt: p.UpdatedAt,
                LastInteractedAt: p.LastInteractedAt,
                Media: mediaList,
                Tags: p.Tags,
                CommentsCount: p.CommentsCount,
                ReactionsCount: p.ReactionsCount,
                ViewsCount: p.ViewsCount,
                SavesCount: p.SavesCount,
                CurrentUserReaction: p.CurrentUserReaction,
                Mentions: mentionsList
            );
        }).ToList();

        var pagedResult = new Paged<PostOverviewDto>(posts, totalCount, pageSize, page);
        return Result<Paged<PostOverviewDto>>.Success(pagedResult, OperationStatusCode.Success);
    }
}
