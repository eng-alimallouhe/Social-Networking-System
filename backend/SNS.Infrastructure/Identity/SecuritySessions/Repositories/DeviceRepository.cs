using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.Repositories;

namespace SNS.Infrastructure.Identity.SecuritySessions.Repositories;

public class DeviceRepository : Repository<Device>
{
    public DeviceRepository(SNSDbContext context) : base(context)
    {
    }
}
