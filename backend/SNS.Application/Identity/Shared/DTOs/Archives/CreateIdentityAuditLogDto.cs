using SNS.Domain.Identity.ArchiveManagement.Enums;

namespace SNS.Application.Identity.Shared.DTOs.Archives;

public sealed record CreateIdentityArchiveDto(
    Guid UserId, 
    string OldIdentifier, 
    string NewIdentifier, 
    IdentityType IdentityType);
