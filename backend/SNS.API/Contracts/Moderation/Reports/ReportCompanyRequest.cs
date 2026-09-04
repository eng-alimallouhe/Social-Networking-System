using SNS.Domain.Moderation.Enums;

namespace SNS.API.Contracts.Moderation.Reports;

public sealed record ReportCompanyRequest(
    ViolationReason ViolationReason,
    string? AdditionalDetails);
