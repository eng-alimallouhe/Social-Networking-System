using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Shared.Abstractions.Specifications;
using System.Linq.Expressions;

namespace SNS.Domain.Identity.SecuritySessions.Specifications;

public class CurrentSecuritySessionByDeviceIdAndUserIdSpecification : ISingleEntitySpecification<SecuritySession>
{
    public Expression<Func<SecuritySession, bool>> Criteria { get; }

    public List<string> Includes => [];

    public Expression<Func<SecuritySession, object>>? OrderBy => null;

    public Expression<Func<SecuritySession, object>>? OrderByDescending => null;

    public CurrentSecuritySessionByDeviceIdAndUserIdSpecification(Guid userId, Guid DeviceId)
    {
        Criteria = session => session.UserId == userId && session.DeviceId == DeviceId && session.LogoutAt == null;
    }
}
