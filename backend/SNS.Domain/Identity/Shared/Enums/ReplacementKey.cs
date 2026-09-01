using System.Text.Json.Serialization;

namespace SNS.Domain.Identity.Shared.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReplacementKey
{
    Device,
    IpAddress,
    UserName,
    RedirectUrl,
    Browser,
    Code,
    LogoUrl,
    OccuredDate,
    City,
    Country,
    NewEmail,
    NewRecoveryEmail,
    Longitude,
    Latitude, 
    OldRole, 
    NewRole,
    RecipientName,
    ProjectName,
    ProjectOwnerName,
    ProjectOwnerProfileImageUrl,
    InvitationUrl,
    InvitedUserName,
    Status
}