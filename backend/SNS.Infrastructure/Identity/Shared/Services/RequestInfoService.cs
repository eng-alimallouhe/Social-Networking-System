using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using MaxMind.GeoIP2.Responses;
using Microsoft.AspNetCore.Http;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Shared.Enums;
using UAParser;

namespace SNS.Infrastructure.Identity.Shared.Services;

public class RequestInfoService : IRequestInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Parser _uaParser;
    private readonly DatabaseReader _reader;

    // 🚀 أسرار قبالية التوسع: كاش لكل عمليات التحليل والقراءة في الـ Request الحالي
    private ClientInfo? _cachedClientInfo;
    private string? _cachedIpAddress;
    private CityResponse? _cachedCityResponse; // كاش الـ GeoIP الحاسم!
    private bool _geoLookupAttempted = false;  // لمنع إعادة المحاولة في حال لم يجد الـ IP

    public RequestInfoService(
        IHttpContextAccessor httpContextAccessor,
        DatabaseReader reader)
    {
        _httpContextAccessor = httpContextAccessor;
        _uaParser = Parser.GetDefault();
        _reader = reader;
    }

    public SupportedLanguage Language
    {
        get
        {
            var header = _httpContextAccessor.HttpContext?.Request.Headers["Accept-Language"].ToString();

            if (string.IsNullOrWhiteSpace(header)) return SupportedLanguage.English;

            if (header.StartsWith("ar", StringComparison.OrdinalIgnoreCase))
                return SupportedLanguage.Arabic;

            return SupportedLanguage.English;
        }
    }

    public string IpAddress
    {
        get
        {
            if (_cachedIpAddress != null) return _cachedIpAddress;

            var context = _httpContextAccessor.HttpContext;
            if (context == null) return "0.0.0.0";

            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var headerValue))
            {
                var header = headerValue.ToString();
                if (!string.IsNullOrWhiteSpace(header))
                {
                    _cachedIpAddress = header.Split(',')[0].Trim();
                    return _cachedIpAddress;
                }
            }

            _cachedIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            return _cachedIpAddress;
        }
    }

    public string UserAgent
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            return context == null ? "Unknown" : context.Request.Headers["User-Agent"].ToString();
        }
    }

    private ClientInfo ParsedClientInfo
    {
        get
        {
            if (_cachedClientInfo != null) return _cachedClientInfo;
            _cachedClientInfo = _uaParser.Parse(UserAgent);
            return _cachedClientInfo;
        }
    }

    // 🎯 دالة الكاش المساعدة للـ GeoIP - تقرأ من الداتا بيز مرة واحدة فقط!
    private CityResponse? ParsedGeoInfo
    {
        get
        {
            if (_geoLookupAttempted) return _cachedCityResponse;
            _geoLookupAttempted = true;

            if (string.IsNullOrWhiteSpace(IpAddress) || IpAddress == "127.0.0.1" || IpAddress == "::1" || IpAddress == "0.0.0.0")
            {
                return null;
            }

            try
            {
                _cachedCityResponse = _reader.City(IpAddress);
                return _cachedCityResponse;
            }
            catch (AddressNotFoundException) { return null; }
            catch (GeoIP2Exception) { return null; }
        }
    }

    public string DeviceName
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                if (context.Request.Headers.TryGetValue("X-DeviceName-Model", out var customModel))
                {
                    var modelStr = customModel.ToString();
                    if (!string.IsNullOrWhiteSpace(modelStr)) return modelStr;
                }

                if (context.Request.Headers.TryGetValue("Sec-CH-UA-Model", out var model))
                {
                    var exactModel = model.ToString().Trim('"');
                    if (!string.IsNullOrWhiteSpace(exactModel)) return exactModel;
                }
            }

            var family = ParsedClientInfo.Device.Family;
            return string.IsNullOrWhiteSpace(family) || family == "Other" ? "Unknown" : family;
        }
    }

    public Guid DeviceId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                if (context.Request.Headers.TryGetValue("X-Device-Id", out var deviceId))
                {
                    var idStr = deviceId.ToString();
                    if (!string.IsNullOrWhiteSpace(idStr) && Guid.TryParse(idStr, out var guid)) return guid;
                }
            }
            return Guid.Empty;
        }
    }

    public string Browser
    {
        get
        {
            var family = ParsedClientInfo.UA.Family;
            return string.IsNullOrWhiteSpace(family) || family == "Other" ? "Unknown" : family;
        }
    }

    public string Country
    {
        get
        {
            if (IpAddress == "127.0.0.1" || IpAddress == "::1") return "Localhost";
            return ParsedGeoInfo?.Country.Name ?? "Unknown";
        }
    }

    public double Latitude => ParsedGeoInfo?.Location.Latitude ?? 0.0;

    public double Longitude => ParsedGeoInfo?.Location.Longitude ?? 0.0;

    public string City
    {
        get
        {
            if (IpAddress == "127.0.0.1" || IpAddress == "::1") return "Localhost";
            return ParsedGeoInfo?.City.Name ?? "Unknown";
        }
    }

    public string FingerprintHash
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                if (context.Request.Headers.TryGetValue("X-Fingerprint-Hash", out var fingerprintHash))
                {
                    var hashStr = fingerprintHash.ToString();
                    if (!string.IsNullOrWhiteSpace(hashStr)) return hashStr;
                }
            }
            return "Unknown";
        }
    }

    public string DeviceToken
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                if (context.Request.Headers.TryGetValue("X-Device-Token", out var deviceToken))
                {
                    var tokenStr = deviceToken.ToString();
                    if (!string.IsNullOrWhiteSpace(tokenStr)) return tokenStr;
                }
            }
            return "Unknown";
        }
    }

    public string DeviceVendor
    {
        get
        {
            var family = ParsedClientInfo.Device.Family;
            return string.IsNullOrWhiteSpace(family) || family == "Other" ? "Unknown" : family;
        }
    }

    public string OperatingSystem
    {
        get
        {
            var family = ParsedClientInfo.OS.Family;
            return string.IsNullOrWhiteSpace(family) || family == "Other" ? "Unknown" : family;
        }
    }

    public string DeviceModel
    {
        get
        {
            var model = ParsedClientInfo.Device.Model;
            return string.IsNullOrWhiteSpace(model) || model == "Other" ? "Unknown" : model;
        }
    }
}
