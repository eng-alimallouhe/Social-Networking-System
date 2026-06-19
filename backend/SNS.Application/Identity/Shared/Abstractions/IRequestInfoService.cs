using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Shared.Abstractions;

public interface IRequestInfoService
{
    /// <summary>
    /// Gets the two-letter ISO language code for the current request.
    /// 
    /// This value is typically resolved from the "Accept-Language" header 
    /// and is used to localize validation errors and response messages.
    /// </summary>
    SupportedLanguage Language { get; }

    /// <summary>
    /// Gets the IP address of the client making the request.
    /// 
    /// This value is used for audit logging and security monitoring.
    /// </summary>
    string IpAddress { get; }

    /// <summary>
    /// Gets the raw User-Agent string provided by the client.
    /// 
    /// This value contains information about the client's operating system and browser.
    /// </summary>
    string UserAgent { get; }

    /// <summary>
    /// Gets the name of the device or operating system used by the client.
    /// 
    /// This value is parsed from the <see cref="UserAgent"/>.
    /// </summary>
    string DeviceName { get; }


    Guid DeviceId { get; }

    /// <summary>
    /// Gets the name and version of the browser used by the client.
    /// 
    /// This value is parsed from the <see cref="UserAgent"/>.
    /// </summary>
    string Browser { get; }

    /// <summary>
    /// Gets the country name or code where the request originated.
    /// 
    /// This value is typically resolved via GeoIP lookup based on <see cref="IpAddress"/>.
    /// </summary>
    string Country { get; }

    double Latitude { get; }

    double Longitude { get; }

    string City { get; }

    string FingerprintHash { get; }

    string DeviceToken { get; }

    string DeviceVendor { get; }

    string OperatingSystem { get; }

    string DeviceModel { get; }
}
