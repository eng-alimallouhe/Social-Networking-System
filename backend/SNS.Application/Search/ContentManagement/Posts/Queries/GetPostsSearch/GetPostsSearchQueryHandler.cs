using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Posts.PostMentions.Contracts;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.ContentManagement.Posts.Queries.GetPostsSearch;

internal sealed class GetPostsSearchQueryHandler
    : IQueryHandler<GetPostsSearchQuery, SearchResult<PostOverviewDto>>
{
    private readonly IPostSearchService _postSearchService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetPostsSearchQueryHandler(
        IPostSearchService postSearchService,
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _postSearchService = postSearchService;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<SearchResult<PostOverviewDto>>> Handle(
        GetPostsSearchQuery request,
        CancellationToken cancellationToken)
    {
        var searchResult = await _postSearchService.SearchAsync(request, cancellationToken);

        if (!searchResult.Hits.Any())
        {
            return Result<SearchResult<PostOverviewDto>>.Success(new SearchResult<PostOverviewDto>
            {
                Hits = new List<SearchHit<PostOverviewDto>>(),
                Total = searchResult.Total
            }, OperationStatusCode.Success);
        }

        var postIds = searchResult.Hits.Select(h => h.Document.Id).ToList();
        var currentProfileId = _currentUserService.ProfileId;

        var rawPosts = await _dbContext.Posts
            .AsNoTracking()
            .Where(p => postIds.Contains(p.Id))
            .Select(p => new
            {
                p.Id,
                AuthorId = p.Author.Id,
                AuthorFullName = p.Author.FullName,
                AuthorSpecialization = p.Author.Specialization,
                AuthorProfilePictureKey = p.Author.ProfilePictureObjectKey,

                p.CommunityId,
                CommunityType = p.Community != null ? p.Community.Type : (SNS.Domain.ContentManagement.Communities.Enums.CommunityType?)null,
                CommunityName = p.Community != null ? p.Community.Name : null,
                CommunityLogoKey = p.Community != null ? p.Community.LogoObjectKey : null,

                p.Title,
                p.Content,
                p.CreatedAt,
                p.UpdatedAt,
                p.LastInteractedAt,
                Media = p.Media.OrderBy(m => m.Order).Select(m => new { m.ObjectKey, m.Order, m.Type }).ToList(),
                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
                CommentsCount = p.Comments.Count(c => c.IsActive),
                ReactionsCount = p.Reactions.Count(),
                ViewsCount = p.Views.Count(),
                SavesCount = p.SavedPosts.Count(),
                CurrentUserReaction = currentProfileId.HasValue ? p.Reactions.Where(r => r.ReactorId == currentProfileId.Value).Select(r => (ReactionType?)r.Type).FirstOrDefault() : null,

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
            CurrentUserReaction: p.CurrentUserReaction,
            Mentions: p.Mentions.Select(m => new PostMentionDto(
                m.ProfileId,
                m.DisplayName,
                m.ProfilePictureKey != null && urlMap.TryGetValue(m.ProfilePictureKey, out var mentionPicUrl) ? mentionPicUrl : null
            )).ToList()
        )).ToList();

        var orderedHits = searchResult.Hits
            .Select(hit =>
            {
                var postDto = posts.FirstOrDefault(p => p.Id == hit.Document.Id);
                return postDto != null ? new SearchHit<PostOverviewDto>(postDto, hit.Score) : null;
            })
            .Where(h => h != null)
            .Select(h => h!)
            .ToList();

        return Result<SearchResult<PostOverviewDto>>.Success(new SearchResult<PostOverviewDto>
        {
            Hits = orderedHits,
            Total = searchResult.Total
        }, OperationStatusCode.Success);
    }
}
