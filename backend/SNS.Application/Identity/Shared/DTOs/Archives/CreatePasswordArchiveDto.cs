namespace SNS.Application.Identity.Shared.DTOs.Archives;

public sealed record CreatePasswordArchiveDto(
    Guid UserId, 
    string PasswordHash);
