using System.Text.Json.Serialization;

namespace SNS.Domain.Identity.SecuritySettings.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SupportTicketStatus
{
    Open,
    InProgress,
    Resolved,
    Rejected,
    Closed
}
