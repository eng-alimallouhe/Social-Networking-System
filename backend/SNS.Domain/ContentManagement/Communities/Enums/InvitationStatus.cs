using System.Text.Json.Serialization;

namespace SNS.Domain.ContentManagement.Communities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InvitationStatus
{
    Pending,
    Accepted,
    Rejected,
    Expired
}
