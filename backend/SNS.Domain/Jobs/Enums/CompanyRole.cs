using System.Text.Json.Serialization;

namespace SNS.Domain.Jobs.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CompanyRole
{
    Owner = 0,
    Manager = 1,
}
