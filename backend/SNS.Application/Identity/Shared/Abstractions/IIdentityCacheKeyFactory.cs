using SNS.Domain.Identity.Users.Enums;

namespace SNS.Application.Identity.Shared.Abstractions;

public interface IIdentityCacheKeyFactory
{
    string GetUserKey(Guid userId);
    string GetSessionKey(Guid sessionId);

    string GetUserSessionsKey(Guid userId);

    string GetOtpKey(Guid userId);

    string GetCoolDownKey(Guid userId);

    string GetAttemptsKey(Guid userId);

    string GetUpdateKey(Guid userId, UpdateType type);

    string GetUserActivationChanlageKey(Guid userId);
}
