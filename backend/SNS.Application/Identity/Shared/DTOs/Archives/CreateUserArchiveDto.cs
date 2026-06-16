using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Identity.Shared.DTOs.Archives;

public sealed record CreateUserArchiveDto(
    Guid UserId, 
    ActionType ActionType,
    Guid PerformedBy,
    Dictionary<ReplacementKey, string>? Parameters = null,
    string? Reason = null);
