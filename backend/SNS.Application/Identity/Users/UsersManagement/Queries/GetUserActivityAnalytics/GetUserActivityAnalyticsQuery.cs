using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserActivityAnalytics;
#region 🔍 Query Request

// الاستعلام الحصين الذي يستقبل معاملات الفلترة الذكية
public sealed record GetUserActivityAnalyticsQuery(
    Guid? TargetUserId,           // null = حسابي الشخصي | قيمة = حساب مستهدف للمدير
    DateTime? FromDate = null,     // بداية الفترة للمنحنى والتوزيع
    DateTime? ToDate = null,       // نهاية الفترة للمنحنى والتوزيع
    string PeriodUnit = "Day"      // الوحدة الزمنية: "Day", "Month", "Year"
) : IQuery<UserActivityAnalyticsResult>;

#endregion