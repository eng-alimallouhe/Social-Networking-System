using System.Text.Json.Serialization;

namespace SNS.Domain.Moderation.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TicketStatus
{
    Pending = 1,      
    Resolved_Approved = 2,
    Resolved_Dismissed = 3
}
