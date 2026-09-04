using SNS.Domain.Moderation.Enums;

namespace SNS.API.Contracts.Moderation.Reports;

public sealed record ReportJobRequest(
    ViolationReason ViolationReason,
    string? AdditionalDetails);
