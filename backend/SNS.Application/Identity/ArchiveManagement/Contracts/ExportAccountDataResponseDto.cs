namespace SNS.Application.Identity.ArchiveManagement.Contracts;

public sealed record ExportAccountDataResponseDto(
    Guid RequestId,
    string Status,
    DateTime CreatedAt);