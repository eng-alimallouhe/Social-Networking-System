using System.Text.Json.Serialization;

namespace SNS.Domain.Identity.Users.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UpdateType
{
    Email, 
    Password,
    Register,
    RecoveryEmail
}
