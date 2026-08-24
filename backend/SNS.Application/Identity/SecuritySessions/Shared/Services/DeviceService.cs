using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;


namespace SNS.Application.Identity.SecuritySessions.Shared.Services;

public sealed class DeviceService : IDeviceService
{
    private readonly IRepository<Device> _deviceRepo;
    private readonly IApplicationDbContext _dbContext;


    public DeviceService(
        IRepository<Device> deviceRepo,
        IApplicationDbContext dbContext)
    {
        _deviceRepo = deviceRepo;
        _dbContext = dbContext;
    }

    public async Task<(Guid deviceId, bool isDeviceTrusted)> GetOrCreateUserDeviceAsync(DeviceCreateDto deviceCreateDto, CancellationToken cancellationToken = default)
    {
        var device = await _dbContext
            .Devices
            .Where(d => d.DeviceToken == deviceCreateDto.DeviceToken || d.FingerprintHash == deviceCreateDto.FingerprintHash)
            .Select(d => new
            {
                Id = d.Id,
                IsTrusted = d.IsTrusted
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (device != null)
        {
            return (device.Id, device.IsTrusted);
        }

        var newDevice = Device.Create(
            userId: deviceCreateDto.UserId,
            deviceToken: deviceCreateDto.DeviceToken,
            friendlyName: deviceCreateDto.FriendlyName,
            browser: deviceCreateDto.Browser,
            operatingSystem: deviceCreateDto.OperatingSystem,
            deviceVendor: deviceCreateDto.DeviceVendor,
            deviceModel: deviceCreateDto.DeviceModel,
            fingerprintHash: deviceCreateDto.FingerprintHash,
            isTrusted: deviceCreateDto.IsTrusted);

        _deviceRepo.Add(newDevice);

        return (newDevice.Id, newDevice.IsTrusted);
    }
}
