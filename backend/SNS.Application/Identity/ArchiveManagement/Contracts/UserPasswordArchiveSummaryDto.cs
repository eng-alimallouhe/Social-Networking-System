namespace SNS.Application.Identity.ArchiveManagement.Contracts;

public sealed record UserPasswordArchiveSummaryDto(
    Guid Id,
    DateTime ChangedAt);