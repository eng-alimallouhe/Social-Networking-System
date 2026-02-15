using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using Microsoft.AspNetCore.Http;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Security;
using UAParser;

namespace SNS.Infrastructure.Services.Common;

public class RequestInfoService : IRequestInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Parser _uaParser;
    private readonly DatabaseReader _reader;

    public RequestInfoService(
        IHttpContextAccessor httpContextAccessor,
        DatabaseReader reader)
    {
        _httpContextAccessor = httpContextAccessor;
        _uaParser = Parser.GetDefault();
        _reader = reader;
    }


    public string Language
    {
        get
        {
            var langFromHeader = GetAcceptLanguage();
            if (!string.IsNullOrWhiteSpace(langFromHeader))
                return langFromHeader;

            return "en";
        }
    }


    public string IpAddress
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                return "0.0.0.0";
            }

            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var header = context.Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrWhiteSpace(header))
                {
                    return header.Split(',')[0].Trim();
                }
            }
            return context.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        }
    }

    
    public string UserAgent
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                return "Unknown";
            }

            var uaString = context.Request.Headers["User-Agent"].ToString();

            return uaString; 
        }
    }

    public string Device
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                return "Unknown";
            }

            var uaString = context.Request.Headers["User-Agent"].ToString();

            var clientInfo = _uaParser.Parse(uaString);

            var device = string.IsNullOrWhiteSpace(clientInfo.OS.Family) ? "Unknown" : clientInfo.OS.Family;

            return device;
        }
    }

    public string Browser
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                return "Unknown";
            }

            var uaString = context.Request.Headers["User-Agent"].ToString();

            var clientInfo = _uaParser.Parse(uaString);

            var device = string.IsNullOrWhiteSpace(clientInfo.OS.Family) ? "Unknown" : clientInfo.UA.Family;

            return device;
        }
    }

    public string Country
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                return "Unknown";
            }

            var ip = "";

            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var header = context.Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrWhiteSpace(header))
                {
                    ip = header.Split(',')[0].Trim();
                }
            }
            else
            {
                ip = context.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            }

            return GetCountryFromIpAsync(ip);
        }
    }



    private string? GetAcceptLanguage()
    {
        var header = _httpContextAccessor.HttpContext?
            .Request.Headers["Accept-Language"].ToString();

        if (string.IsNullOrWhiteSpace(header))
            return null;

        return header.Split(',').FirstOrDefault()?.Split('-').FirstOrDefault();
    }



    private string GetCountryFromIpAsync(string ipAddress)
    {
        // 1. Handle Loopback addresses immediately
        if (string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "127.0.0.1" || ipAddress == "::1")
        {
            return "Localhost";
        }

        try
        {
            // 2. Query the database (In-Memory operation, extremely fast)
            var response = _reader.Country(ipAddress);

            // Return the English name or default fallback
            return response.Country.Name ?? "Unknown";
        }
        catch (AddressNotFoundException)
        {
            // This exception occurs for private IPs (e.g., 192.168.1.1) or reserved ranges
            // that are not present in the public GeoIP database.
            return "Unknown";
        }
        catch (GeoIP2Exception)
        {
            // General library errors (e.g., malformed IP format)
            return "Unknown";
        }
    }
}