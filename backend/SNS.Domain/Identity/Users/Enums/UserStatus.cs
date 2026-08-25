using System.Text.Json.Serialization;

namespace SNS.Domain.Identity.Users.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserStatus
{
    Active,
    Suspended,
    PermanentlyBanned,
    Deactivated
}