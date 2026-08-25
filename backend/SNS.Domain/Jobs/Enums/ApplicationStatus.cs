using System.Text.Json.Serialization;

namespace SNS.Domain.QA.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ApplicationStatus
{
    Pending,
    Reviewed,
    Accepted,
    Rejected,
    Withdrawn
}
