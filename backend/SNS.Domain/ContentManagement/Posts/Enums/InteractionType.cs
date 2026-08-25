using System.Text.Json.Serialization;

namespace SNS.Domain.ContentManagement.Posts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InteractionType
{
    Like,
    Comment,
    NotInterested
}
