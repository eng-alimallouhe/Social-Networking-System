using System.Text.Json.Serialization;

namespace SNS.Domain.ContentManagement.Communities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ModerationPolicy
{
    Open,          
    ReviewRequired,
    Strict         
}
