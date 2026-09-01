using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Posts.PostMentions.Contracts;
using SNS.Application.ContentManagement.Posts.Posts.Abstractions;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Search.ContentManagement.Posts.Queries;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.ContentManagement.Shared.Enums;

namespace SNS.Application.ContentManagement.Posts.Posts.Services;

internal sealed class FeedFallbackService : IFeedFallbackService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public FeedFallbackService(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<PostOverviewDto>> GetFallbackFeedAsync(
        FeedRequestParameter parameter,
        int pageSize = 30,
        CancellationToken cancellationToken = default)
    {
        var topicIds = parameter.Topics
            .Select(x => x.TopicId)
            .ToList();

        var rawPosts = await _dbContext.Posts
            .AsNoTracking()
            .Where(p =>
                !parameter.ExcludedPostsIds.Contains(p.Id) &&
                !parameter.ExcludedProfilesIds.Contains(p.AuthorId) &&
                p.CreatedAt >= parameter.StartDate &&
                (
                    parameter.FollowedProfilesIds.Contains(p.AuthorId) ||
                    (p.CommunityId.HasValue &&
                     parameter.CommunitiesIds.Contains(p.CommunityId.Value)) ||
                    p.PostTopics.Any(pt => topicIds.Contains(pt.TopicId))
                ))
            .OrderByDescending(p => p.CreatedAt)
            .Take(pageSize)
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

                Media = p.Media
                    .OrderBy(m => m.Order)
                    .Select(m => new { m.ObjectKey, m.Order, m.Type })
                    .ToList(),

                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
                CommentsCount = p.Comments.Count(c => c.IsActive),
                ReactionsCount = p.Reactions.Count(),
                ViewsCount = p.Views.Count(),
                SavesCount = p.SavedPosts.Count(),
                CurrentUserReaction = p.Reactions.Where(r => r.ReactorId == parameter.ProfileId).Select(r => (ReactionType?)r.Type).FirstOrDefault(),

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

        // Collect all distinct storage keys to resolve temporary URLs in parallel
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

        return rawPosts.Select(p => new PostOverviewDto(
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
    }
}
