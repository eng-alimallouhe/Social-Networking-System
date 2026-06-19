namespace SNS.Application.Identity.Users.AdminAcions.Queries.GetUserActivityAnalytics;



#region 🎁 Data Transfer Objects (DTOs)

// 1️⃣ كرت الهوية الشخصية للمستخدم
public sealed record UserProfileHeaderDto(
    string FullName,
    string Specialization,
    string? ProfilePictureUrl);

// 2️⃣ بطاقات العدادات الإجمالية التاريخية
public sealed record LifetimeCountersDto(
    int TotalPosts,
    int TotalReactionsCasted,
    int TotalComments,
    int TotalProblemsPublished,
    int TotalSolutionsPublished,
    int TotalVotesCasted,
    int ReputationPoints,
    int TotalProjectsCreated,
    int TotalProjectsJoined);

// 3️⃣ كائن المنحنى الزمني (نقطة بيانية واحدة تحتوي على التاريخ وعدد الحركات)
public sealed record ActivityGraphPointDto(
    DateTime PeriodLabel, // قد يكون تاريخ يوم "2026-06-14" أو اسم شهر "June 2026"
    int ActionsCount);

// 4️⃣ قسم توزيع التفاعل (النسب المئوية الجاهزة للـ Pie Chart)
public sealed record InteractionDistributionDto(
    double ContentManagementPercentage,  // بوستات وتعليقات
    double QaDiscussionsPercentage,     // مشاكل وحلول
    double ProjectsCommunitiesPercentage // مشاريع ومجتمعات
);

// 5️⃣ سجل آخر 5 حركات حية قام بها المستخدم
public sealed record RecentActivityLogDto(
    Guid ActivityId,
    ActivityType ActivityType,
    DateTime OccurredAt);


// 🏆 الـ DTO العملاق والشامل الذي سيستقبله علي بالفرونت إند بضربة واحدة
public sealed record UserActivityAnalyticsResult(
    UserProfileHeaderDto UserProfile,
    LifetimeCountersDto LifetimeStats,
    IReadOnlyCollection<ActivityGraphPointDto> ActivityGraph,
    InteractionDistributionDto InteractionDistribution,
    IReadOnlyCollection<RecentActivityLogDto> RecentActivities);

public enum ActivityType
{
    CreatePost,
    CreateComment,
    CreateProblem,
    CreateSolution,
    ReactingOnPost,
    ReactingOnComment,
    VoteForProblem,
    VoteForSolution,
    CreateProject,
    ContributeInProject
}

#endregion