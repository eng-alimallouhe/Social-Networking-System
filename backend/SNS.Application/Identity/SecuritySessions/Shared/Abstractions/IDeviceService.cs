using SNS.Application.Identity.SecuritySessions.Shared.Contracts;

namespace SNS.Application.Identity.SecuritySessions.Shared.Abstractions;

public interface IDeviceService
{
    Task<(Guid deviceId, bool isDeviceTrusted)> GetOrCreateUserDeviceAsync(DeviceCreateDto deviceCreateDto, CancellationToken cancellationToken = default);
}
