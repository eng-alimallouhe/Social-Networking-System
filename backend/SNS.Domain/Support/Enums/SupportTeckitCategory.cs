using System.Text.Json.Serialization;

namespace SNS.Domain.Support.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SupportTeckitCategory
{
    Technical, 
    Billing, 
    General
}
