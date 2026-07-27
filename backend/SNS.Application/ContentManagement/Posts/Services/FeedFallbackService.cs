using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Posts.Abstractions;
using SNS.Application.ContentManagement.Posts.Contracts;
using SNS.Application.Search.ContentManagement.Posts.Queries;
using SNS.Application.Shared.Abstractions.Data;

internal sealed class FeedFallbackService : IFeedFallbackService
{
    private readonly IApplicationDbContext _dbContext;

    public FeedFallbackService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PostOverviewDto>> GetFallbackFeedAsync(
    FeedRequestParameter parameter,
    int pageSize = 30,
    CancellationToken cancellationToken = default)
    {
        var topicIds = parameter.Topics
            .Select(x => x.TopicId)
            .ToList();

        return await _dbContext.Posts
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
            .Select(p => new PostOverviewDto(
                Id: p.Id,
                AuthorId: p.AuthorId,
                AuthorName: p.Author.FullName,
                AuthorSpecialization: p.Author.Specialization,
                AuthorProfilePictureUrl: p.Author.ProfilePictureObjectKey,

                CommunityId: p.CommunityId,
                CommunityType: p.Community != null ? p.Community.Type : null,
                CommunityName: p.Community != null ? p.Community.Name : null,
                CommunityLogoUrl: p.Community != null ? p.Community.LogoObjectKey : null,

                Title: p.Title,
                Content: p.Content,

                CreatedAt: p.CreatedAt,
                UpdatedAt: p.UpdatedAt,
                LastInteractedAt: p.LastInteractedAt,

                FirstMediaUrl: p.Media
                    .OrderBy(m => m.Order)
                    .Select(m => m.ObjectKey)
                    .FirstOrDefault() ?? string.Empty,

                MediaCount: p.Media.Count(),

                Tags: p.PostTags
                    .Select(pt => pt.Tag.Name)
                    .ToList(),

                CommentsCount: p.Comments.Count(),

                ReactionsCount: p.Reactions.Count(),

                ViewsCount: p.Views.Count(),

                SavesCount: p.SavedPosts.Count()
            ))
            .ToListAsync(cancellationToken);
    }
}