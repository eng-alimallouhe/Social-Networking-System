namespace SNS.Domain.Identity.SecuritySessions.Abstractions;

public interface ISessionRepository
{
    Task UpdateSessionLastSeenAsync(Guid sessionId, DateTime lastSeen, CancellationToken cancellationToken = default);
}