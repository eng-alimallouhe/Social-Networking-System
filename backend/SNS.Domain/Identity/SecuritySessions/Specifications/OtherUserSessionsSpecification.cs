using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Shared.Abstractions.Specifications;
using System.Linq.Expressions;

namespace SNS.Domain.Identity.SecuritySessions.Specifications;

public class OtherUserSessionsSpecification : ISpecification<SecuritySession>
{
    public Expression<Func<SecuritySession, bool>> Criteria { get; }

    public List<string> Includes {  get; }

    public bool IsTrackingEnabled => true;

    public Expression<Func<SecuritySession, object>>? OrderBy => null;

    public Expression<Func<SecuritySession, object>>? OrderByDescending => null;

    public int? Skip => null;

    public int? Take => null;

    public OtherUserSessionsSpecification(Guid userId, Guid currentSessionId)
    {
        Criteria = ss => 
            ss.UserId == userId && ss.Id != currentSessionId && ss.LogoutAt == null;

        Includes = new List<string>()
        {
            nameof(SecuritySession.RefreshTokens)
        };
    }
}
