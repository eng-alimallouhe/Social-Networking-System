using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserInformation;

public sealed record UserInformationResult(
    string UserName,
    string RoleName,
    string Email,
    SupportedLanguage PreferredLanguage,
    DateTime LastPasswordChange,
    string? Location,
    string? LastActiveLocation,
    bool HasActiveDataDownloadRequest);
