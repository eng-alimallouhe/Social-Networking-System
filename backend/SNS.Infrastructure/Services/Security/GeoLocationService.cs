using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using SNS.Application.Abstractions.Security;


namespace SNS.Infrastructure.Services.Security;

public class GeoLocationService : IGeoLocationService
{
    // The reader is thread-safe and injected as a Singleton
    private readonly DatabaseReader _reader;

    public GeoLocationService(DatabaseReader reader)
    {
        _reader = reader;
    }

    public Task<string> GetCountryFromIpAsync(string ipAddress)
    {
        // 1. Handle Loopback addresses immediately
        if (string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "127.0.0.1" || ipAddress == "::1")
        {
            return Task.FromResult("Localhost");
        }

        try
        {
            // 2. Query the database (In-Memory operation, extremely fast)
            var response = _reader.Country(ipAddress);

            // Return the English name or default fallback
            return Task.FromResult(response.Country.Name ?? "Unknown");
        }
        catch (AddressNotFoundException)
        {
            // This exception occurs for private IPs (e.g., 192.168.1.1) or reserved ranges
            // that are not present in the public GeoIP database.
            return Task.FromResult("Unknown");
        }
        catch (GeoIP2Exception)
        {
            // General library errors (e.g., malformed IP format)
            return Task.FromResult("Unknown");
        }
    }
}