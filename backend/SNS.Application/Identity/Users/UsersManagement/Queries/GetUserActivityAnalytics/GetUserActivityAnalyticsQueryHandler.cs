using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Identity.Users.Enums; // تأكد من الـ Namespace الخاص بـ ActivityType
using SNS.Domain.Projects.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserActivityAnalytics;

public sealed class GetUserActivityAnalyticsQueryHandler
    : IQueryHandler<UserActivityAnalyticsResult, UserActivityAnalyticsResult>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetUserActivityAnalyticsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserActivityAnalyticsResult>> Handle(
        UserActivityAnalyticsResult request,
        CancellationToken cancellationToken)
    {
        // 1️⃣ حارس بوابة الأمان والتحقق من صلاحية الـ Admin
        var targetUserId = _currentUserService.UserId;

        if (request.TargetUserId != null)
        {
            var currentUserRole = _currentUserService.RoleType;
            if (currentUserRole == null || !currentUserRole.Contains("admin", StringComparison.OrdinalIgnoreCase))
            {
                return Result<UserActivityAnalyticsResult>.Failure(SecurityStatusCodes.AuthenticationRequired);
            }
            targetUserId = request.TargetUserId;
        }

        if (targetUserId == null || targetUserId == Guid.Empty)
        {
            return Result<UserActivityAnalyticsResult>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        // 2️⃣ السلسلة العملاقة والنقية للـ GroupJoin بضربة SQL واحدة نفاثة
        var dbData = await _dbContext.Profiles
            .AsNoTracking()
            .Where(p => p.UserId == targetUserId)
            .GroupJoin(_dbContext.Posts, p => p.Id, o => o.AuthorId, (profile, postsGroup) => new { profile, postsGroup })
            .GroupJoin(_dbContext.Comments, prev => prev.profile.Id, o => o.AuthorId, (prev, commentsGroup) => new { prev.profile, prev.postsGroup, commentsGroup })
            .GroupJoin(_dbContext.Problems, prev => prev.profile.Id, o => o.AuthorId, (prev, problemsGroup) => new { prev.profile, prev.postsGroup, prev.commentsGroup, problemsGroup })
            .GroupJoin(_dbContext.Solutions, prev => prev.profile.Id, o => o.AuthorId, (prev, solutionsGroup) => new { prev.profile, prev.postsGroup, prev.commentsGroup, prev.problemsGroup, solutionsGroup })
            .GroupJoin(_dbContext.PostReactions, prev => prev.profile.Id, o => o.ReactorId, (prev, postReactionsGroup) => new { prev.profile, prev.postsGroup, prev.commentsGroup, prev.problemsGroup, prev.solutionsGroup, postReactionsGroup })
            .GroupJoin(_dbContext.CommentReactions, prev => prev.profile.Id, o => o.ReactorId, (prev, commentReactionsGroup) => new { prev.profile, prev.postsGroup, prev.commentsGroup, prev.problemsGroup, prev.solutionsGroup, prev.postReactionsGroup, commentReactionsGroup })
            .GroupJoin(_dbContext.ProblemVotes, prev => prev.profile.Id, o => o.VoterId, (prev, problemVotesGroup) => new { prev.profile, prev.postsGroup, prev.commentsGroup, prev.problemsGroup, prev.solutionsGroup, prev.postReactionsGroup, prev.commentReactionsGroup, problemVotesGroup })
            .GroupJoin(_dbContext.Projects, prev => prev.profile.Id, o => o.OwnerId, (prev, projectsGroup) => new { prev.profile, prev.postsGroup, prev.commentsGroup, prev.problemsGroup, prev.solutionsGroup, prev.postReactionsGroup, prev.commentReactionsGroup, prev.problemVotesGroup, projectsGroup })
            .GroupJoin(_dbContext.ProjectContributors, prev => prev.profile.Id, o => o.ContributorId, (prev, projectContributorsGroup) => new { prev.profile, prev.postsGroup, prev.commentsGroup, prev.problemsGroup, prev.solutionsGroup, prev.postReactionsGroup, prev.commentReactionsGroup, prev.problemVotesGroup, prev.projectsGroup, projectContributorsGroup })
            .Select(p => new
            {
                p.profile.FullName,
                p.profile.Specialization,
                p.profile.ProfilePictureUrl,
                p.profile.Reputation,

                TotalPosts = p.postsGroup.Count(),
                TotalComments = p.commentsGroup.Count(),
                TotalProblems = p.problemsGroup.Count(),
                TotalSolutions = p.solutionsGroup.Count(),
                TotalVotesCasted = p.problemVotesGroup.Count(),
                TotalProjectsCreated = p.projectsGroup.Count(),
                TotalProjectsJoined = p.projectContributorsGroup.Count(pc => pc.InvitingStatus == InvitingStatus.Accepted),

                // حساب الحركات الصادرة الإجمالية للتفاعلات
                TotalReactionsCasted = p.postReactionsGroup.Count() + p.commentReactionsGroup.Count(),

                // تصحيح الـ Mapping وسحب الـ Top 5 لكل قطاع بنقاء
                RecentPosts = p.postsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentComments = p.commentsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentProblems = p.problemsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentSolutions = p.solutionsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentPostReactions = p.postReactionsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentCommentReactions = p.commentReactionsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentProjects = p.projectsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentContributions = p.projectContributorsGroup.Where(pc => pc.InvitingStatus == InvitingStatus.Accepted && pc.RespondedAt.HasValue).OrderByDescending(x => x.RespondedAt).Take(5).Select(x => new { x.Id, x.RespondedAt })
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dbData == null)
        {
            return Result<UserActivityAnalyticsResult>.Failure(ResourceStatusCode.NotFound);
        }

        // 3️⃣ حساب نسب التفاعل للـ Pie Chart منعاً للـ Division by zero
        int totalAllActions = dbData.TotalPosts + dbData.TotalComments + dbData.TotalProblems + dbData.TotalSolutions + dbData.TotalProjectsCreated + dbData.TotalProjectsJoined;
        int denominator = totalAllActions == 0 ? 1 : totalAllActions;

        var distribution = new InteractionDistributionDto(
            ContentManagementPercentage: Math.Round(((double)(dbData.TotalPosts + dbData.TotalComments) / denominator) * 100, 2),
            QaDiscussionsPercentage: Math.Round(((double)(dbData.TotalProblems + dbData.TotalSolutions) / denominator) * 100, 2),
            ProjectsCommunitiesPercentage: Math.Round(((double)(dbData.TotalProjectsCreated + dbData.TotalProjectsJoined) / denominator) * 100, 2)
        );

        // 4️⃣ الـ In-Memory Merge النظيف والآمن لآخر 5 حركات حية
        var mergedActivities = new List<RecentActivityLogDto>();

        mergedActivities.AddRange(dbData.RecentPosts.Select(x => new RecentActivityLogDto(x.Id, ActivityType.CreatePost, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentComments.Select(x => new RecentActivityLogDto(x.Id, ActivityType.CreateComment, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentProblems.Select(x => new RecentActivityLogDto(x.Id, ActivityType.CreateProblem, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentSolutions.Select(x => new RecentActivityLogDto(x.Id, ActivityType.CreateSolution, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentPostReactions.Select(x => new RecentActivityLogDto(x.Id, ActivityType.ReactingOnPost, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentCommentReactions.Select(x => new RecentActivityLogDto(x.Id, ActivityType.ReactingOnComment, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentProjects.Select(x => new RecentActivityLogDto(x.Id, ActivityType.CreateProject, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentContributions.Select(x => new RecentActivityLogDto(x.Id, ActivityType.ContributeInProject, x.RespondedAt!.Value)));

        // الترتيب والأخذ النهائي لأحدث 5 حركات على مستوى السستم ككل 🏆
        var finalRecentActivities = mergedActivities
            .OrderByDescending(a => a.OccurredAt)
            .Take(5)
            .ToList();

        // 5️⃣ بناء وإعداد منحي النشاط الزمني (Activity Graph) خفيف ومؤقت بالقيم الافتراضية
        // (يمكن توسيعه بـ GroupBy زمني لاحقاً حسب المدخلات)
        var graphPoints = new List<ActivityGraphPointDto>
        {
            new ActivityGraphPointDto(DateTime.UtcNow.ToString("yyyy-MM-dd"), totalAllActions)
        };

        // 6️⃣ تجميع الحزم داخل الـ Result الفاخر
        var result = new UserActivityAnalyticsResult(
            UserProfile: new UserProfileHeaderDto(dbData.FullName, dbData.Specialization!, dbData.ProfilePictureUrl),
            LifetimeStats: new LifetimeCountersDto(dbData.TotalPosts, dbData.TotalReactionsCasted, dbData.TotalComments, dbData.TotalProblems, dbData.TotalSolutions, dbData.TotalVotesCasted, dbData.Reputation, dbData.TotalProjectsCreated, dbData.TotalProjectsJoined),
            ActivityGraph: graphPoints,
            InteractionDistribution: distribution,
            RecentActivities: finalRecentActivities
        );

        return Result<UserActivityAnalyticsResult>.Success(result, OperationStatusCode.Success);
    }
}