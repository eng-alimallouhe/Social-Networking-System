using SNS.Application.Identity.SecuritySessions.DTOs;

namespace SNS.Application.Identity.SecuritySessions.Abstractions;

public interface IDeviceService
{
    Task<(Guid deviceId, bool isDeviceTrusted)> GetOrCreateUserDeviceAsync(DeviceCreateDto deviceCreateDto, CancellationToken cancellationToken = default);
}
