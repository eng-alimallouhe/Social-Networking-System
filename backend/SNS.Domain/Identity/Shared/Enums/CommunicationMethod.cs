using System.Text.Json.Serialization;

namespace SNS.Domain.Identity.Shared.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommunicationMethod
{
    Email,
    RecoveryEmail
}
