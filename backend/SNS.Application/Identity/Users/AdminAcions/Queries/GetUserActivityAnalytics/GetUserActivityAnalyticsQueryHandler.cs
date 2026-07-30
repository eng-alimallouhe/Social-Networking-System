using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Projects.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.AdminAcions.Queries.GetUserActivityAnalytics;

/// <summary>
/// Handles the execution of <see cref="GetUserActivityAnalyticsQuery"/> to compute user activity analytics.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Enforces administrative authorization check.
/// 2. Executes read-only queries (<c>AsNoTracking</c>) aggregating user activity counters across posts, comments, Q&amp;A, reactions, votes, and projects.
/// 3. Computes percentage distribution across content, Q&amp;A, and project interactions.
/// 4. Groups date points in-memory based on requested period unit (Day, Month, or Year).
/// 5. Maps profile picture keys to public storage URLs.
/// </remarks>
public sealed class GetUserActivityAnalyticsQueryHandler
    : IQueryHandler<GetUserActivityAnalyticsQuery, UserActivityAnalyticsResult>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetUserActivityAnalyticsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<UserActivityAnalyticsResult>> Handle(
        GetUserActivityAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        // 1️⃣ حارس بوابة الأمان: التأكد من أن المستخدم الحالي هو Admin
        var currentUserRole = _currentUserService.RoleType;
        if (currentUserRole == null || !currentUserRole.Contains("admin", StringComparison.OrdinalIgnoreCase))
        {
            return Result<UserActivityAnalyticsResult>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var targetUserId = request.TargetUserId; // 👈 استخدام الحساب المستهدف المرسل بالطلب صراحة

        // إعداد الفلاتر الزمنية الافتراضية للجراف إن لم ترسل (مثلاً آخر 30 يوماً)
        var fromDate = request.FromDate ?? DateTime.UtcNow.AddDays(-30);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        // 2️⃣ السلسلة العملاقة والنقية لجلب البيانات الأساسية من الـ DB مع فلاتر الجراف المضمنة
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
            .GroupJoin(
                _dbContext.ProjectContributors, 
                prev => prev.profile.Id, 
                o => o.ContributorId, 
                (prev, projectContributorsGroup) => new { prev.profile, prev.postsGroup, prev.commentsGroup, prev.problemsGroup, prev.solutionsGroup, prev.postReactionsGroup, prev.commentReactionsGroup, prev.problemVotesGroup, prev.projectsGroup, projectContributorsGroup })
            .Select(p => new
            {
                p.profile.FullName,
                p.profile.Specialization,
                p.profile.ProfilePictureObjectKey,
                p.profile.Reputation,

                TotalPosts = p.postsGroup.Count(),
                TotalComments = p.commentsGroup.Count(),
                TotalProblems = p.problemsGroup.Count(),
                TotalSolutions = p.solutionsGroup.Count(),
                TotalVotesCasted = p.problemVotesGroup.Count(),
                TotalProjectsCreated = p.projectsGroup.Count(),
                TotalProjectsJoined = p.projectContributorsGroup.Count(pc => pc.InvitingStatus == InvitingStatus.Accepted),

                TotalReactionsCasted = p.postReactionsGroup.Count() + p.commentReactionsGroup.Count(),

                // سحب آخر 5 حركات حية
                RecentPosts = p.postsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentComments = p.commentsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentProblems = p.problemsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentSolutions = p.solutionsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentPostReactions = p.postReactionsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentCommentReactions = p.commentReactionsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentProjects = p.projectsGroup.OrderByDescending(x => x.CreatedAt).Take(5).Select(x => new { x.Id, x.CreatedAt }),
                RecentContributions = p.projectContributorsGroup.Where(pc => pc.InvitingStatus == InvitingStatus.Accepted && pc.RespondedAt.HasValue).OrderByDescending(x => x.RespondedAt).Take(5).Select(x => new { x.Id, x.RespondedAt }),

                // 📈 جلب كافة التواريخ ضمن النطاق المحدد لتغذية الجراف بشكل ديناميكي
                GraphPostDates = p.postsGroup.Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate).Select(x => x.CreatedAt),
                GraphCommentDates = p.commentsGroup.Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate).Select(x => x.CreatedAt),
                GraphProblemDates = p.problemsGroup.Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate).Select(x => x.CreatedAt),
                GraphSolutionDates = p.solutionsGroup.Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate).Select(x => x.CreatedAt),
                GraphProjectDates = p.projectsGroup.Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate).Select(x => x.CreatedAt),
                GraphContributorDates = p.projectContributorsGroup.Where(pc => pc.InvitingStatus == InvitingStatus.Accepted && pc.RespondedAt >= fromDate && pc.RespondedAt <= toDate).Select(x => x.RespondedAt!.Value)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dbData == null)
        {
            return Result<UserActivityAnalyticsResult>.Failure(ResourceStatusCode.NotFound);
        }

        int totalAllActions = dbData.TotalPosts + dbData.TotalComments + dbData.TotalProblems + dbData.TotalSolutions + dbData.TotalProjectsCreated + dbData.TotalProjectsJoined;
        int denominator = totalAllActions == 0 ? 1 : totalAllActions;

        var distribution = new InteractionDistributionDto(
            ContentManagementPercentage: Math.Round(((double)(dbData.TotalPosts + dbData.TotalComments) / denominator) * 100, 2),
            QaDiscussionsPercentage: Math.Round(((double)(dbData.TotalProblems + dbData.TotalSolutions) / denominator) * 100, 2),
            ProjectsCommunitiesPercentage: Math.Round(((double)(dbData.TotalProjectsCreated + dbData.TotalProjectsJoined) / denominator) * 100, 2)
        );

        var mergedActivities = new List<RecentActivityLogDto>();
        mergedActivities.AddRange(dbData.RecentPosts.Select(x => new RecentActivityLogDto(x.Id, ActivityType.CreatePost, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentComments.Select(x => new RecentActivityLogDto(x.Id, ActivityType.CreateComment, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentProblems.Select(x => new RecentActivityLogDto(x.Id, ActivityType.CreateProblem, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentSolutions.Select(x => new RecentActivityLogDto(x.Id, ActivityType.CreateSolution, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentPostReactions.Select(x => new RecentActivityLogDto(x.Id, ActivityType.ReactingOnPost, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentCommentReactions.Select(x => new RecentActivityLogDto(x.Id, ActivityType.ReactingOnComment, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentProjects.Select(x => new RecentActivityLogDto(x.Id, ActivityType.CreateProject, x.CreatedAt)));
        mergedActivities.AddRange(dbData.RecentContributions.Select(x => new RecentActivityLogDto(x.Id, ActivityType.ContributeInProject, x.RespondedAt!.Value)));

        var finalRecentActivities = mergedActivities
            .OrderByDescending(a => a.OccurredAt)
            .Take(5)
            .ToList();

        // 5️⃣ 📈 التعبئة الديناميكية الفولاذية للجراف الزمني (In-Memory Grouping) بناءً على الـ DateTime الجديد
        var allGraphDates = new List<DateTime>();
        allGraphDates.AddRange(dbData.GraphPostDates);
        allGraphDates.AddRange(dbData.GraphCommentDates);
        allGraphDates.AddRange(dbData.GraphProblemDates);
        allGraphDates.AddRange(dbData.GraphSolutionDates);
        allGraphDates.AddRange(dbData.GraphProjectDates);
        allGraphDates.AddRange(dbData.GraphContributorDates);

        var graphPoints = new List<ActivityGraphPointDto>();

        if (request.PeriodUnit.Equals("Month", StringComparison.OrdinalIgnoreCase))
        {
            graphPoints = allGraphDates
                .GroupBy(d => new DateTime(d.Year, d.Month, 1))
                .Select(g => new ActivityGraphPointDto(g.Key, g.Count()))
                .OrderBy(g => g.PeriodLabel)
                .ToList();
        }
        else if (request.PeriodUnit.Equals("Year", StringComparison.OrdinalIgnoreCase))
        {
            graphPoints = allGraphDates
                .GroupBy(d => new DateTime(d.Year, 1, 1))
                .Select(g => new ActivityGraphPointDto(g.Key, g.Count()))
                .OrderBy(g => g.PeriodLabel)
                .ToList();
        }
        else
        {
            graphPoints = allGraphDates
                .GroupBy(d => d.Date)
                .Select(g => new ActivityGraphPointDto(g.Key, g.Count()))
                .OrderBy(g => g.PeriodLabel)
                .ToList();
        }

        if (!graphPoints.Any())
        {
            graphPoints.Add(new ActivityGraphPointDto(DateTime.UtcNow.Date, 0));
        }

        var result = new UserActivityAnalyticsResult(
            UserProfile: new UserProfileHeaderDto(dbData.FullName, dbData.Specialization!, dbData.ProfilePictureObjectKey != null? _fileStorageService.GetFilePublicUrl(dbData.ProfilePictureObjectKey) : null),
            LifetimeStats: new LifetimeCountersDto(dbData.TotalPosts, dbData.TotalReactionsCasted, dbData.TotalComments, dbData.TotalProblems, dbData.TotalSolutions, dbData.TotalVotesCasted, dbData.Reputation, dbData.TotalProjectsCreated, dbData.TotalProjectsJoined),
            ActivityGraph: graphPoints,
            InteractionDistribution: distribution,
            RecentActivities: finalRecentActivities
        );

        return Result<UserActivityAnalyticsResult>.Success(result, OperationStatusCode.Success);
    }
}