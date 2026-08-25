using System.Text.Json.Serialization;

namespace SNS.Domain.Identity.Shared.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SendPurpose
{
    UserDeactivated = 0, // this when the user deactivated you should add this keys: UserName, Device, IpAddress, Browser, OccuredDate
    UserVerification = 1, // this when the user was registered and we want to verify his email, you should add this keys: UserName, Code, RedirectUrl, LogoObjectKey 
    LoginTwoFactor = 2, // this use when the user want to login and the TFA is enabled we send the code to the user here you should add this keys: UserName, Code, RedirectUrl, LogoObjectKey
    PasswordReset = 3, // this is an email that is sent when the user want to change his passwored for public gate and we ant to verify the person that request to change is the user by sending an OTP code to the email you should add this keys: UserName, Code, RedirectUrl, LogoObjectKey
    EmailChangeVerification = 4, //  this is an email that is sent when the user want to change his email and we ant to verify it you should add this keys: UserName, Code, RedirectUrl, LogoObjectKey
    RecoveryEmailChangeVerification = 4, // this is an email that is sent when the user want to change his recovery email and we ant to verify it you should add this keys: UserName, Code, RedirectUrl, LogoObjectKey
    PasswordChangedAlert = 10, // this is an alert is send when the user change his passwored successfully 
    EmailChangedAlert = 11, // this is an alert send when the email for the user was changed 
    LoginAlert = 12, // this is an alert for the user when there is an login to his account from a device you should add this keys: UserName, Device, OccuredDate, RedirectUrl (to redirect the user when he wan't the logged en), Country, City, IpAddress, Longitude, Latitude (the lat and long for like an button or link when the user click on it we redirect it to google maps on this location 
    LoginWithSecurityCodeAlert = 13, // this is an alert for the user when there is a login for his account and this login was by Recovey Code
    UserDeleting = 14, // this when the user want to deactivate his account, you should add this keys: UserName, Code, RedirectUrl, LogoObjectKey 
    HighRiskLogin = 15, // this is an alert for the user when there is an attempt to login and the attempt is high risk attempt you should add this keys: UserName, Device, OccuredDate, Country, City, IpAddress, Longitude, Latitude (the lat and long for like an button or link when the user click on it we redirect it to google maps on this location
    RoleChangedAlert = 16, // this is an alert we send it when the admin change the user role you should add this keys: OldRole, NewRole 
    ExportDataCompleted = 17
}
