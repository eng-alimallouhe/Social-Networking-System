using SNS.Domain.Identity.ArchiveManagement.Enums;

namespace SNS.Application.Identity.ArchiveManagement.Contracts;

public sealed record UserIdentityArchiveSummaryDto(
    Guid Id,
    string OldIdentifier,
    string NewIdentifier,
    IdentityType Type,
    DateTime CreatedAt);
