using System.Text.Json.Serialization;

namespace SNS.Domain.Jobs.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SalaryTyp
{
    Monthly,
    Yearly,
    Hourly,
    Negotiable
}
