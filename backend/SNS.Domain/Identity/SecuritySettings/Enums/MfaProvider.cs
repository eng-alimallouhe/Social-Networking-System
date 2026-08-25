using System.Text.Json.Serialization;

namespace SNS.Domain.Identity.SecuritySettings.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MfaProvider
{
    None = 0,
    RecoveryEmail = 1,
    Email = 2,
    AuthenticatorApp = 3,
    Passkey = 4,
}