using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Identity.ArchiveManagement.Contracts;

public sealed record UserArchiveSummaryDto(
    Guid Id,
    ActionType Type,
    string Reason,
    Guid? PerformedById,
    string PerformedByUserName,
    Dictionary<ReplacementKey, string>? Parameters,
    DateTime CreatedAt);