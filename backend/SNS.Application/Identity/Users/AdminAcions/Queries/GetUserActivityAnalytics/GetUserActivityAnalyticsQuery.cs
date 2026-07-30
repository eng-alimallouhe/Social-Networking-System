using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.AdminAcions.Queries.GetUserActivityAnalytics;
#region 🔍 Query Request

/// <summary>
/// Represents a query to retrieve user activity analytics for administrative review.
/// </summary>
/// <param name="TargetUserId">The unique identifier of the target user whose activity analytics are being requested.</param>
/// <param name="FromDate">Optional start date boundary for the activity timeline and distribution.</param>
/// <param name="ToDate">Optional end date boundary for the activity timeline and distribution.</param>
/// <param name="PeriodUnit">The grouping unit for the activity graph ("Day", "Month", or "Year").</param>
public sealed record GetUserActivityAnalyticsQuery(
    Guid TargetUserId,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string PeriodUnit = "Day"
) : IQuery<UserActivityAnalyticsResult>;

#endregion