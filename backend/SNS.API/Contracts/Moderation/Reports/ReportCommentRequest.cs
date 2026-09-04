using SNS.Domain.Moderation.Enums;

namespace SNS.API.Contracts.Moderation.Reports;

public sealed record ReportCommentRequest(
    ViolationReason ViolationReason,
    string? AdditionalDetails);
