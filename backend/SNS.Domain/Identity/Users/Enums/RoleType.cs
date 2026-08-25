using System.Text.Json.Serialization;

namespace SNS.Domain.Identity.Users.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoleType
{
    Admin = 0,
    User = 1,
    Moderator = 2,
    Guest = 3,
    Support = 4,
    Ghost = 5
}
