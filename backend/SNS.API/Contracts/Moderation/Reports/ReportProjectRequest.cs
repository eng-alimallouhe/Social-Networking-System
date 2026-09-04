using SNS.Domain.Moderation.Enums;

namespace SNS.API.Contracts.Moderation.Reports;

public sealed record ReportProjectRequest(
    ViolationReason ViolationReason,
    string? AdditionalDetails);
