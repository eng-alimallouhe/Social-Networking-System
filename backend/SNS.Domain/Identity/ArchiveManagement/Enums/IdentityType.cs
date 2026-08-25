using System.Text.Json.Serialization;

namespace SNS.Domain.Identity.ArchiveManagement.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IdentityType
{
    Email,
    RecoveryEmail
}
