using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Posts.Abstractions;
using SNS.Application.ContentManagement.Posts.Contracts;
using SNS.Application.Search.ContentManagement.Posts.Queries;
using SNS.Application.Shared.Abstractions.Data;

namespace SNS.Application.ContentManagement.Posts.Services;

public class FeedBackgroundService : IFeedBackgroundService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPostCacheService _postCacheService;

    public FeedBackgroundService(
        IApplicationDbContext dbContext,
        IPostCacheService postCacheService)
    {
        _dbContext = dbContext;
        _postCacheService = postCacheService;
    }

    public async Task ComputeAndCacheUserFeedAsync(Guid profileId, FeedRequestParameter feedParams)
    {
        bool acquired = await _postCacheService.TryLockFeedBuildingAsync(profileId);

        if (!acquired)
        {
            return;
        }

        try{
            // 1. جلب معايير المستخدم (الكود الخاص بك تماماً)
            
            var followedProfiles =
                feedParams.FollowedProfilesIds.ToHashSet();

            var excludedPosts =
                feedParams.ExcludedPostsIds.ToHashSet();

            var excludedProfiles =
                feedParams.ExcludedProfilesIds.ToHashSet();

            var communities =
                feedParams.CommunitiesIds.ToHashSet();

            var userTopicsDict =
                feedParams.Topics.ToDictionary(x => x.TopicId, x => x.Score);

            var userTagsDict =
                feedParams.Tags.ToDictionary(x => x.TagId, x => x.Score);

            var interestedTopics =
                userTopicsDict.Keys.ToHashSet();

            // 2. جلب البوستات المرشحة بناءً على معايير الفيد (Candidates Generation)
            var rawCandidates = await _dbContext.Posts
                .Where(p =>
                    // ألا يكون البوست محذوفاً أو من ضمن المستبعدين (المشاهدة أو المحظورين)
                    !feedParams.ExcludedPostsIds.Contains(p.Id) &&
                    !feedParams.ExcludedProfilesIds.Contains(p.AuthorId) &&
                    (
                        feedParams.FollowedProfilesIds.Contains(p.AuthorId) ||
                        (p.CommunityId.HasValue &&
                         feedParams.CommunitiesIds.Contains(p.CommunityId.Value)) ||
                        p.PostTopics.Any(pt =>
                            feedParams.Topics
                                .Select(t => t.TopicId)
                                .Contains(pt.TopicId))
                    )
                )
                .OrderByDescending(p => p.CreatedAt) // جلب الأحدث كبداية للفلترة
                .Take(500) // أخذ عينة كافية للرانك
                .Select(p => new
                {
                    Id = p.Id,
                    CreatedAt = p.CreatedAt,
                    AuthorId = p.AuthorId,
                    Topics = p.PostTopics.Select(pt => new
                    {
                        TopicId = pt.TopicId,
                        Confidence = pt.Confidence ?? 1
                    }),
                    Tags = p.PostTags.Select(pt => new
                    {
                        TagId = pt.TagId,
                        Confidence = pt.Confidence ?? 1
                    }),
                    LikesCount = p.Reactions.Count(),   // تأكد من أسماء علاقات التفاعلات لديك
                    ViewsCount = p.Views.Count()
                })
                .ToListAsync();

            // 3. تطبيق خوارزمية الـ Ranking (حساب السكور لكل بوست)
            var now = DateTime.UtcNow;
            var rankedItems = new List<FeedItemModel>();

            foreach (var post in rawCandidates)
            {
                double hoursOld = (now - post.CreatedAt).TotalHours;
                // معادلة Time Decay لتخفيض سكور البوست القديم وزيادة الحديث
                double timeScore = 1.0 / (Math.Pow(hoursOld + 2, 1.5));

                double engagementScore = (post.LikesCount * 2.0) + (post.ViewsCount * 0.1);

                // بونص إذا كان من شخص يتابعه المستخدم
                double followBonus =
                    followedProfiles.Contains(post.AuthorId)
                        ? 30
                        : 0;

                double topicInterestBonus = 0;
                foreach (var postTopic in post.Topics)
                {
                    if (userTopicsDict.TryGetValue(postTopic.TopicId, out double userTopicScore))
                    {
                        topicInterestBonus += userTopicScore * postTopic.Confidence * 10.0;
                    }
                }

                double tagInterestBonus = 0;

                foreach (var postTag in post.Tags)
                {
                    if (userTagsDict.TryGetValue(
                        postTag.TagId,
                        out double userTagScore))
                    {
                        tagInterestBonus +=
                            userTagScore *
                            postTag.Confidence *
                            10;
                    }
                }

                double finalScore = (engagementScore * timeScore) + followBonus + tagInterestBonus + topicInterestBonus;

                rankedItems.Add(new FeedItemModel(post.Id, finalScore));
            }

            // 4. ترتيب العناصر تنازلياً حسب السكور وحفظها دفعة واحدة في Redis عبر الخدمة التي أنشأتها
            var topSortedFeed = rankedItems
                .OrderByDescending(x => x.Score)
                .Take(300) // أخذ أعلى 300 وتخزينهم
                .ToList();

            if (topSortedFeed.Any())
            {
                await _postCacheService.SetProfileFeedAsync(profileId, topSortedFeed, cancellationToken: default);
            }
        }
        finally
        {
            await _postCacheService.UnlockFeedBuildingAsync(profileId);
        }
    }
}