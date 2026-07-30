namespace SNS.Application.Identity.Users.AdminAcions.Queries.GetUserActivityAnalytics;


#region 🎁 Data Transfer Objects (DTOs)

/// <summary>
/// Represents user profile header details for activity analytics view.
/// </summary>
/// <param name="FullName">The full name of the user profile.</param>
/// <param name="Specialization">The user's professional specialization.</param>
/// <param name="ProfilePictureUrl">Optional URL to the user's profile avatar.</param>
public sealed record UserProfileHeaderDto(
    string FullName,
    string Specialization,
    string? ProfilePictureUrl);

/// <summary>
/// Represents lifetime cumulative activity statistics and engagement metrics.
/// </summary>
/// <param name="TotalPosts">Total posts created by the user.</param>
/// <param name="TotalReactionsCasted">Total reactions given on posts and comments.</param>
/// <param name="TotalComments">Total comments authored by the user.</param>
/// <param name="TotalProblemsPublished">Total Q&amp;A problems published.</param>
/// <param name="TotalSolutionsPublished">Total solutions submitted.</param>
/// <param name="TotalVotesCasted">Total votes cast on problems and solutions.</param>
/// <param name="ReputationPoints">Total reputation score earned by the user.</param>
/// <param name="TotalProjectsCreated">Total projects created by the user.</param>
/// <param name="TotalProjectsJoined">Total projects joined by the user.</param>
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

/// <summary>
/// Represents a single data point in the user activity timeline graph.
/// </summary>
/// <param name="PeriodLabel">The timestamp or period label for the data point.</param>
/// <param name="ActionsCount">The number of activity actions performed during the period.</param>
public sealed record ActivityGraphPointDto(
    DateTime PeriodLabel,
    int ActionsCount);

/// <summary>
/// Represents percentage distribution of user interaction across application domains.
/// </summary>
/// <param name="ContentManagementPercentage">Percentage of activity in posts and comments.</param>
/// <param name="QaDiscussionsPercentage">Percentage of activity in Q&amp;A discussions.</param>
/// <param name="ProjectsCommunitiesPercentage">Percentage of activity in projects and communities.</param>
public sealed record InteractionDistributionDto(
    double ContentManagementPercentage,
    double QaDiscussionsPercentage,
    double ProjectsCommunitiesPercentage
);

/// <summary>
/// Represents a recent activity event log entry.
/// </summary>
/// <param name="ActivityId">The unique identifier of the activity record.</param>
/// <param name="ActivityType">The type of action performed.</param>
/// <param name="OccurredAt">The timestamp when the activity occurred.</param>
public sealed record RecentActivityLogDto(
    Guid ActivityId,
    ActivityType ActivityType,
    DateTime OccurredAt);

/// <summary>
/// Represents comprehensive user activity analytics result containing header info, lifetime stats, timeline graph, distribution metrics, and recent activity logs.
/// </summary>
/// <param name="UserProfile">The user profile header information.</param>
/// <param name="LifetimeStats">Aggregated lifetime activity statistics.</param>
/// <param name="ActivityGraph">Timeline graph data points.</param>
/// <param name="InteractionDistribution">Interaction distribution percentage breakdown.</param>
/// <param name="RecentActivities">Collection of recent activity log entries.</param>
public sealed record UserActivityAnalyticsResult(
    UserProfileHeaderDto UserProfile,
    LifetimeCountersDto LifetimeStats,
    IReadOnlyCollection<ActivityGraphPointDto> ActivityGraph,
    InteractionDistributionDto InteractionDistribution,
    IReadOnlyCollection<RecentActivityLogDto> RecentActivities);

/// <summary>
/// Specifies the type of activity action performed by a user.
/// </summary>
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