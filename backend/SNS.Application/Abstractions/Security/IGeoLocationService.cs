namespace SNS.Application.Abstractions.Security;

/// <summary>
/// Defines a contract for resolving geographic location information
/// from an IP address.
/// 
/// This abstraction shields the application layer from the specific
/// implementation details of GeoIP lookups (whether via database or API).
/// </summary>
public interface IGeoLocationService
{
    /// <summary>
    /// Resolves the country name from the given IP address.
    /// </summary>
    /// <param name="ipAddress">The IPv4 or IPv6 address to lookup.</param>
    /// <returns>
    /// The English name of the country (e.g., "United States") or "Unknown Location"
    /// if resolution fails.
    /// </returns>
    Task<string> GetCountryFromIpAsync(string ipAddress);
}