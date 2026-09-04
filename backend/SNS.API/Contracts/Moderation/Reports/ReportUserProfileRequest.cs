using SNS.Domain.Moderation.Enums;

namespace SNS.API.Contracts.Moderation.Reports;

public sealed record ReportUserProfileRequest(
    ViolationReason ViolationReason,
    string? AdditionalDetails);
