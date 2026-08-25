using System.Text.Json.Serialization;

namespace SNS.Domain.Identity.Notifications.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationSource
{
    System,         
    Content,        
    Community,      
    Projects,       
    Problems,       
    Requests,       
    Security        
}