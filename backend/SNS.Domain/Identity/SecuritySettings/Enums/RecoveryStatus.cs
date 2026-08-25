using System.Text.Json.Serialization;

namespace SNS.Domain.Identity.SecuritySettings.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RecoveryStatus
{
    Pending,
    Verified,
    Unverified,
    Completed,
    Rejected,
    NeedsMoreInfo
}
