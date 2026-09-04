using SNS.Domain.Moderation.Enums;

namespace SNS.API.Contracts.Moderation.Reports;

public sealed record ReportRatingRequest(
    ViolationReason ViolationReason,
    string? AdditionalDetails);
