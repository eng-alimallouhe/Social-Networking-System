using System.Text.Json.Serialization;

namespace SNS.Domain.Support.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TicketStatus
{
    Open = 1,
    Pending = 2,
    Resolved = 3,
    Closed = 4
}